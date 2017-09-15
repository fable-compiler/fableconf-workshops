module Dashboard.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types
open Fulma.Layouts
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.BulmaClasses
open Fulma.Components
open System

// let replyMedia =
//     Media.media [ ]
//         [ Media.left [ ]
//             [ Image.image [ Image.is64x64 ]
//                 [ img [ Src "/img/logo.svg" ] ] ]
//           Media.content [ ]
//             [ Field.field_div [ ]
//                 [ Control.control_div [ ]
//                     [ Textarea.textarea [ ]
//                         [ ] ] ]
//               Level.level [ ]
//                 [ Level.left [ ]
//                     [ Level.item [ ]
//                         [ Button.button [ Button.isInfo ]
//                             [ str "Submit" ] ] ]
//                   Level.right [ ]
//                     [ Level.item [ ]
//                         [ str "Pres Ctrl + Enter to submit" ] ] ] ] ]

// let root model dispatch =
//     Container.container [ ]
//         [ Section.section [ Section.customClass Bulma.Properties.Alignment.HasTextCentered ]
//             [ Heading.h4 [ Heading.is4 ]
//                 [ str "What is the average wingspeed of an unladen swallow?" ] ]
//           Media.media [ ]
//                 [ Media.left [ ]
//                     [ Image.image [ Image.is64x64 ]
//                         [ img [ Src "/img/logo.svg" ] ] ]
//                   Media.content [ ]
//                     [ ] ]
//           replyMedia ]

let questionView (question : Shared.Types.Question) =
    Logger.log question.CreatedAt
    let createdAt = DateTime.Parse(question.CreatedAt)
    let url = AuthenticatedPage.Question >> AuthPage >> toHash

    Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.is64x64 ]
                    [ img [ Src ("/img/avatars/" + question.Author.Avatar)  ] ] ]
              Media.content [ ]
                [ a [ Href (url question.Id) ]
                    [ str question.Title ]
                  Level.level [ ]
                    [ Level.left [ ] [ ] // Needed to force the level right aligment
                      Level.right [ ]
                        [ Level.item [ ]
                            [ Help.help [ ]
                                [ str (sprintf "Asked by %s %s, %s"
                                                    question.Author.Firstname
                                                    question.Author.Surname
                                                    (createdAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]

let root model dispatch =
    Container.container [ ]
        [ Section.section [ ]
            [ Heading.p [ Heading.is5 ]
                [ str "Latest questions" ] ]
          Columns.columns [ Columns.isCentered ]
            [ Column.column [ Column.Width.isTwoThirds ]
                (model.Questions |> List.map questionView ) ] ]
