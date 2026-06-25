public enum ErrorType
{
    /// <summary>
    /// A general failure occurred.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Input data failed business or schema validation rules (e.g., invalid email format).
    /// </summary>
    Validation = 1,

    /// <summary>
    /// A generic problem occurred during processing; often used for unexpected domain logic hurdles.
    /// </summary>
    Problem = 2,

    /// <summary>
    /// The requested resource was not found in the system.
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// The request conflicts with the current state of the server (e.g., duplicate unique keys).
    /// </summary>
    Conflict = 4,

    /// <summary>
    /// Authentication is required or has failed. The user is not logged in.
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// Authenticated user does not have permission to access this specific resource or action.
    /// </summary>
    Forbidden = 6,

    /// <summary>
    /// An unhandled exception or critical unexpected error occurred within the application.
    /// </summary>
    Unexpected = 7,

    /// <summary>
    /// The service is temporarily unable to handle the request (e.g., maintenance or overloading).
    /// </summary>
    Unavailable = 8,

    /// <summary>
    /// The request itself is malformed or contains syntax errors that prevent processing.
    /// </summary>
    InvalidRequest = 9,

    /// <summary>
    /// An error occurred in an external 3rd-party service (e.g., Payment Gateway, Email Provider).
    /// </summary>
    ExternalProvider = 10,

    /// <summary>
    /// The action is logically invalid for the entity's current state (e.g., deleting an already shipped order).
    /// </summary>
    InvalidState = 11,

    /// <summary>
    /// The user or client has exceeded their allowed quota or rate limit.
    /// </summary>
    LimitExceeded = 12
}