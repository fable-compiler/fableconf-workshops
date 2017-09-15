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

let replyMedia (fieldValue: StringField) isWaiting dispatch =
    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.is64x64 ]
                [ img [ Src ("/img/avatars/" + LocalStorage.Session.User.Avatar) ] ] ]
          Media.content [ ]
            [ Field.field_div [ ]
                [ yield Control.control_div [ if isWaiting then yield Control.isLoading ]
                    [ Textarea.textarea [ yield Textarea.props [ OnChange (fun ev -> !!ev.target?value |> ChangeReply |> dispatch)
                                                                 OnKeyDown (fun ev ->
                                                                    if ev.ctrlKey && ev.key = "Enter" then
                                                                        dispatch Submit
                                                                 )
                                                                 // We use Ref to empty the text area value
                                                                 // Thanks React and it's "Uncontrolled" element
                                                                 Ref (fun x ->
                                                                    if not (isNull x) then
                                                                        if fieldValue.Value = "" then
                                                                            let textarea = x :?> Browser.HTMLTextAreaElement
                                                                            textarea.value <- ""
                                                                  )
                                                               ]
                                          if isWaiting then yield Textarea.isDisabled ]
                        [ ] ]
                  if fieldValue.Error.IsSome then
                    yield Help.help [ Help.isDanger ]
                            [ str fieldValue.Error.Value ] ]
              Level.level [ ]
                [ Level.left [ ]
                    [ Level.item [ ]
                        [ Button.button [ yield Button.isPrimary
                                          yield Button.onClick (fun _ -> dispatch Submit)
                                          if isWaiting then yield Button.isDisabled ]
                            [ str "Submit" ] ] ]
                  Level.right [ ]
                    [ Level.item [ ]
                        [ str "Press Ctrl + Enter to submit" ] ] ] ] ]

let answerView (answer : Shared.Types.Answer) =
    let createdAt = DateTime.Parse(answer.CreatedAt).ToLocalTime()

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
                                        (createdAt.ToString("yyyy-MM-dd HH:mm:ss"))) ] ] ] ] ] ]


let questionView (question : Shared.Types.Question) answers =
    let createdAt = DateTime.Parse(question.CreatedAt).ToLocalTime()
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
                      replyMedia model.Reply model.IsWaitingReply dispatch ] ] ]
    | None ->
        Container.container [ ]
            [ str "Loading..." ]
