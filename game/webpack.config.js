var path = require("path");
var webpack = require("webpack");
var fableUtils = require("fable-utils");

function resolve(filePath) {
  return path.join(__dirname, filePath)
}

var babelOptions = fableUtils.resolveBabelOptions({
  presets: [["es2015", { "modules": false }]],
  plugins: ["transform-runtime"]
});

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

function getWebpackConfig(bundleName, projFile) {
  projFile = resolve(projFile);
  return {
    devtool: "source-map",
    entry: projFile,
    output: {
      filename: bundleName + '.js',
      path: resolve('./public/build'),
      publicPath: 'build'
    },
    resolve: {
      modules: [resolve("./node_modules/")]
    },
    devServer: {
      contentBase: resolve('./public'),
      port: 8080
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
              extra: {
                // To avoid confusion with shared files, we need
                // to tell Fable what project these files belong too
                projectFile: projFile
              }
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
        }
      ]
    }
  };
}

module.exports = [
  getWebpackConfig("app", "src/App/App.fsproj"),
  getWebpackConfig("worker", "src/Worker/Worker.fsproj"),
]
