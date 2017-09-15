module Question.Show.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Types
open Fulma.Layouts
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.BulmaClasses
open Fulma.Components
open System

[<Pojo>]
type DangerousInnerHtml =
    { __html : string }

let htmlFromMarkdown str = div [ DangerouslySetInnerHTML { __html = Marked.Globals.marked.parse (str) } ] []

let replyMedia =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src "/img/logo.svg" ] ] ]
          Media.content [ ]
            [ Field.field_div [ ]
                [ Control.control_div [ ]
                    [ Textarea.textarea [ ]
                        [ ] ] ]
              Level.level [ ]
                [ Level.left [ ]
                    [ Level.item [ ]
                        [ Button.button [ Button.isPrimary ]
                            [ str "Submit" ] ] ]
                  Level.right [ ]
                    [ Level.item [ ]
                        [ str "Pres Ctrl + Enter to submit" ] ] ] ] ]

let answerView (answer : Shared.Types.Answer) =
    let createdAt = DateTime.Parse(answer.CreatedAt)

    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("img/avatars/" + answer.Author.Avatar) ] ] ]
          Media.content [ ]
            [ Content.content [ ]
                [ htmlFromMarkdown answer.Content ]
              Level.level [ ]
                [ Level.right [ ]
                    [ ]
                  Level.left [ ]
                    [ Level.item [ ]
                        [ Help.help [ ]
                            [ str (sprintf "Answer by %s %s, %s"
                                        answer.Author.Firstname
                                        answer.Author.Surname
                                        (createdAt.ToString("yyyy-MM-dd hh:mm:ss"))) ] ] ] ] ] ]


let questionView (question : Shared.Types.Question) answers =
    let createdAt = DateTime.Parse(question.CreatedAt)
    let url = AuthenticatedPage.Question >> AuthPage >> toHash

    Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.is64x64 ]
                    [ img [ Src ("/img/avatars/" + question.Author.Avatar)  ] ] ]
              Media.content [ ]
                [ yield Content.content [ ]
                            [ htmlFromMarkdown question.Description ]
                  yield Level.level [ ]
                            [ Level.left [ ] [ ] // Needed to force the level right aligment
                              Level.right [ ]
                                [ Level.item [ ]
                                    [ Help.help [ ]
                                        [ str (sprintf "Asked by %s %s, %s"
                                                    question.Author.Firstname
                                                    question.Author.Surname
                                                    (createdAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ]
                  yield! (List.map answerView answers) ] ]

let root model dispatch =
    match model.Question with
    | Some question ->
        Container.container [ ]
            [ Section.section [ ]
                [ Heading.p [ Heading.is5 ]
                    [ str question.Title ] ]
              Columns.columns [ Columns.isCentered ]
                [ Column.column [ Column.Width.isTwoThirds ]
                    [ questionView question model.Answers
                      replyMedia ] ] ]
    | None ->
        Container.container [ ]
            [ str "Loading..." ]
