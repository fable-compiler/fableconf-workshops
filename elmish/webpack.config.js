var path = require("path");
var webpack = require("webpack");
var fableUtils = require("fable-utils");
var nodeExternals = require('webpack-node-externals');
var nodemonPlugin = require('nodemon-webpack-plugin');

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

var babelOptions = fableUtils.resolveBabelOptions({
    presets: [["es2015", { "modules": false }]],
    plugins: ["transform-runtime"]
});


var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

var basicConfig = {
    devtool: "source-map",
    resolve: {
        modules: [
            resolve("./node_modules/")
        ]
    },
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                use: {
                    loader: "fable-loader",
                    options: {
                        babel: babelOptions,
                        define: isProduction ? [] : ["DEBUG"],
                    }
                }
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: babelOptions
                },
            },
            {
                test: /\.sass$/,
                use: [
                    "style-loader",
                    "css-loader",
                    "sass-loader"
                ]
            }
        ]
    }
};

var clientConfig = Object.assign({
    entry: resolve("src/Client/Client.fsproj"),
    output: {
        path: resolve("output/client"),
        filename: "bundle.js"
    },
    devServer: {
        contentBase: resolve('./public'),
        port: 8080
    }
}, basicConfig);

var serverConfig = Object.assign({
    target: "node",
    node: {
        __filename: false,
        __dirname: false
    },
    externals: [nodeExternals()],
    entry: resolve("src/Server/Server.fsproj"),
    output: {
        path: resolve("output/server"),
        filename: "server.js"
    },
    plugins: [
        new nodemonPlugin()
    ]
}, basicConfig);

module.exports = [clientConfig, serverConfig]
