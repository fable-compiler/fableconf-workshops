module Shared.Validation

open System
open Shared.Types

let [<Literal>] FieldRequired = "This field is required"

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
