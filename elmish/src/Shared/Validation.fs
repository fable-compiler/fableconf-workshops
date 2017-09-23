module Shared.Validation

open System
open Shared.Types

let [<Literal>] FieldRequired = "This field is required"

module SignIn =

    let [<Literal>] UserNotFound = "user_not_found"
    let [<Literal>] SignInSuccess = "sign_in_success"

    let verifyEmail email =
        if String.IsNullOrWhiteSpace email then
            Some FieldRequired
        else
            None

    let verifyPassword password =
        if String.IsNullOrWhiteSpace password then
            Some FieldRequired
        else
            None

    let verifySignInData (fields : SignInData) =
        verifyEmail fields.Email = None
        && verifyPassword fields.Password = None

module Question =

    let [<Literal>] QuestionNotFound = "question_not_found"

    module Show =

        let [<Literal>] CreateAnswerSuccess = "create_answer_success"

        let verifyAnswer answer =
            if String.IsNullOrWhiteSpace answer then
                Some "Your answer can't be empty"
            else
                None

        let verifyCreateAnswer (data : CreateAnswer) =
            verifyAnswer data.Content = None

module User =

    let verifyFirstname firstname =
        if String.IsNullOrWhiteSpace firstname then
            Some FieldRequired
        else
            None

    let verifySurname surname =
        if String.IsNullOrWhiteSpace surname then
            Some FieldRequired
        else
            None

    let verifyEmail email =
        if String.IsNullOrWhiteSpace email then
            Some FieldRequired
        else
            None

    let verifyPassword password =
        if String.IsNullOrWhiteSpace password then
            Some FieldRequired
        else
            None

    let verifyPasswordConfirmation confirmation password =
        if String.IsNullOrWhiteSpace confirmation then
            Some FieldRequired
        elif confirmation <> password then
            Some "Doesn't match password"
        else
            None

    let verifyUserUpdate (fields : UserEdit) =
        verifyFirstname fields.Firstname = None
        && verifySurname fields.Surname = None
        && verifyEmail fields.Email = None

    let verifyUserCreate (fields : UserCreate) =
        verifyFirstname fields.Firstname = None
        && verifySurname fields.Surname = None
        && verifyEmail fields.Email = None
        && verifyPassword fields.Password = None
        && verifyPasswordConfirmation fields.PasswordConfirmation fields.Password = None
