



namespace Coherence.Samples.Kien
{
    using Cloud;
    using Runtime;
    public class DialogUI
    {

        public static string GetErrorFromResponse<T>(RequestResponse<T> requestResponse)
        {
            if (requestResponse.Exception is not RequestException requestException)
            {
                return default;
            }

            return requestException.ErrorCode switch
            {
                ErrorCode.InvalidCredentials => "Invalid authentication credentials, please login again.",
                ErrorCode.TooManyRequests => "Too many requests. Please try again in a moment.",
                ErrorCode.ProjectNotFound => "Project not found. Please check that the runtime key is properly setup.",
                ErrorCode.SchemaNotFound => "Schema not found. Please check if the schema currently used by the project matches the one used by the replication server.",
                ErrorCode.RSVersionNotFound => "Replication server version not found. Please check that the version of the replication server is valid.",
                ErrorCode.SimNotFound => "Simulator not found. Please check that the slug and the schema are valid and that the simulator has been uploaded.",
                ErrorCode.MultiSimNotListening => "The multi-room simulator used for this room is not listening on the required ports. Please check your multi-room sim setup.",
                ErrorCode.RoomsSimulatorsNotEnabled => "Simulator not enabled. Please make sure that simulators are enabled in the coherence Dashboard.",
                ErrorCode.RoomsSimulatorsNotUploaded => "Simulator not uploaded. You can use the coherence Hub to build and upload Simulators.",
                ErrorCode.RoomsVersionNotFound => "Version not found. Please make sure that client uses the correct 'sim-slug'.",
                ErrorCode.RoomsSchemaNotFound => "Schema not found. Please check if the schema currently used by the project matches the one used by the replication server.",
                ErrorCode.RoomsRegionNotFound => "Region not found. Please make sure that the selected region is enabled in the Dev Portal.",
                ErrorCode.RoomsInvalidTagOrKeyValueEntry => "Validation of tag and key/value entries failed. Please check if number and size of entries is within limits.",
                ErrorCode.RoomsCCULimit => "Room ccu limit for project exceeded.",
                ErrorCode.RoomsNotFound => "Room not found. Please refresh room list.",
                ErrorCode.RoomsInvalidSecret => "Invalid room secret. Please make sure that the secret matches the one received on room creation.",
                ErrorCode.RoomsInvalidMaxPlayers => "Room Max Players must be a value between 1 and the upper limit configured on the project dashboard.",
                ErrorCode.InvalidMatchMakingConfig => "Invalid matchmaking configuration. Please make sure that the matchmaking feature was properly configured in the Dev Portal.",
                ErrorCode.ClientPermission => "The client has been restricted from accessing this feature. Please check the game services settings on the Dev Portal.",
                ErrorCode.CreditLimit => "Monthly credit limit exceeded. Please check your organization credit usage in the Dev Portal.",
                ErrorCode.InDeployment => "One or more online resources are currently being provisioned. Please retry the request.",
                ErrorCode.FeatureDisabled => "Requested feature is disabled, make sure you enable it in the Game Services section of your coherence Dashboard.",
                ErrorCode.InvalidRoomLimit => "Room max players limit must be between 1 and 100.",
                ErrorCode.LobbyInvalidAttribute => "A specified Attribute is invalid.",
                ErrorCode.LobbyNameTooLong => "Lobby name must be shorter than 64 characters.",
                ErrorCode.LobbyTagTooLong => "Lobby tag must be shorter than 16 characters.",
                ErrorCode.LobbyNotFound => "Requested Lobby wasn't found.",
                ErrorCode.LobbyAttributeSizeLimit => "A specified Attribute has surpassed the allowed limits. Lobby limit: 2048. Player limit: 256. Attribute size is calculated off key length + value length of all attributes combined.",
                ErrorCode.LobbyNameAlreadyExists => "A lobby with this name already exists.",
                ErrorCode.LobbyRegionNotFound => "Specified region for this Lobby wasn't found.",
                ErrorCode.LobbyInvalidSecret => "Invalid secret specified for lobby.",
                ErrorCode.LobbyFull => "This lobby is currently full.",
                ErrorCode.LobbyActionNotAllowed => "You're not allowed to perform this action on the lobby.",
                ErrorCode.LobbyInvalidFilter => "The provided filter is invalid. You can use Filter.ToString to debug the built filter you're sending.",
                ErrorCode.LobbyNotCompatible => "Schema not found. Please check if the schema currently used by the project matches the one used by the replication server.",
                ErrorCode.LobbySimulatorNotEnabled => "Simulator not enabled. Please make sure that simulators are enabled in the coherence Dashboard.",
                ErrorCode.LobbySimulatorNotUploaded => "Simulator not uploaded. You can use the coherence Hub to build and upload Simulators.",
                ErrorCode.LobbyLimit => "You cannot join more than three lobbies simultaneously.",
                ErrorCode.LoginInvalidUsername => "Username given is invalid. Only alphanumeric, dashes and underscore characters are allowed. It must start with a letter and end with a letter/number. No double dash/underscore characters are allowed (-- or __).",
                ErrorCode.LoginInvalidPassword => "Password given is invalid. Password cannot be empty.",
                ErrorCode.RestrictedModeCapReached => "Total user capacity for restricted mode server reached.",
                ErrorCode.LoginDisabled => "This authentication method is disabled.",
                ErrorCode.LoginInvalidApp => "The provided App ID is invalid.",
                ErrorCode.LoginNotFound => "No player account has been linked to the authentication method that was used.",
                ErrorCode.OneTimeCodeExpired => "The one-time code has already expired.",
                ErrorCode.OneTimeCodeNotFound => "The one-time code was not found.",
                ErrorCode.IdentityLimit => "Unique identity limit reached.",
                ErrorCode.IdentityNotFound => "Identity not found.",
                ErrorCode.IdentityRemoval => "Tried to unlink last authentication method from player account.",
                ErrorCode.IdentityTaken => "Identity already linked to another player account.",
                ErrorCode.IdentityTotalLimit => "Maximum allowed identity limit reached.",
                ErrorCode.InvalidConfig => "Invalid configuration. Please make sure that all the necessary information has been provided in coherence Dashboard.",
                ErrorCode.InvalidInput => "Invalid input. Please make sure to provide all required arguments.",
                ErrorCode.PasswordNotSet => "Password has not been set for the player account.",
                ErrorCode.UsernameNotAvailable => "The username is already taken by another player account.",
                _ => requestException.Message,
            };
        }
    }
}
