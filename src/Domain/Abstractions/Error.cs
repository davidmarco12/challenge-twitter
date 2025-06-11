namespace TwitterAPI.Domain.Abstractions
{
    public record Error(string code, string description)
    {
        public static readonly Error FollowError = new("FOLLOW_ERROR", "Follow ERROR");

        public static readonly Error NullValue = new("GENERIC_ERROR.Valor_Nulo", "Un valor nulo fue ingresado");

        public static readonly Error InvalidAPIResponse = new("GENERIC_ERROR.Invalid_API_Response", "La respuesta de la API fue nula o inválida");
    }
}
