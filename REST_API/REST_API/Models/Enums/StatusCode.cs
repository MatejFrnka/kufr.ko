﻿namespace REST_API.Models.Enums
{
    public enum StatusCode
    {
        OK,
        INVALID_REQUEST,
        TOKEN_INVALID,
        TOKEN_INACTIVE,
        TOKEN_EXPIRED,
        DATABASE_ERROR,
        INVALID_EMAIL,
        INVALID_PASSWORD,
        EMPTY_EMAIL,
        EMPTY_PASSWORD,
        EMPTY_NAME,
        EMAIL_ALREADY_EXISTS,
        FORBIDDEN,
        FAIC
    }
}