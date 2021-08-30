namespace ApiPagoMP.Helpers
{
    public static class Constantes
    {
        public const string EMAIL = "EMAIL";
        public const string SOLICITUD = "SOLICITUD";
        public const string MODELO = "MODELO";
        public const string NOMBRE_TITULAR = "NOMBRE_TITULAR";
        public const string ID_USUARIO_OPENPAY = "ID_USUARIO_OPENPAY";
        public const string LLAVE = "llave";
        public const string AUTHORIZATION = "Authorization";
        public const string TELEFONO = "TELEFONO";

        public const string ISO_8859_1 = "iso-8859-1";
        
        //Autenticación Básica
        public const string USUARIO = "USUARIO";
        public const string CONTRASENIA = "CONTRASENIA";
        public const char DOS_PUNTOS = ':';
        public const int ENTERO_CERO = 0;
        public const int ENTERO_UNO = 1;

        //Prefijos
        public const string BASIC_AUTH =  "Basic ";
        public const string BEARER_AUTH = "Bearer ";

        //Conexiones a Bases de datos
        public const string BD_SOFT = "ConexionBDSoft";
        public const string BD_CORP = "ConexionBDCorporativo";


        //secciones
        public const string BASIC_AUTH_KEY = "KeyBasicAuth";
        public const string BASIC_KEY_JWT = "KeyJwt";
        public const string BASIC_TIEMPO_EXP = "tiempo_expiracion";
        public const string BASIC_ISSUER_VALUE = "issuer_value";

        //SAG
        public const string URL_SAG                 = "url_sag";
        public const string URL_SAG_USUARIO         = "usuario";
        public const string URL_SAG_PASS            = "pass";
        public const string URL_SAG_ID_CODERESULT   = "id_codresult";
        
        public const bool CALIFICADO = true;//0 es true
        public const bool NO_CALIFICADO = false;//1 es false
        
        public const string FORMATO_FECHA = "dd/MM/yyyy";
        public const string FORMATO_FECHAHORA = "dd/MM/yyyy HH:mm:ss";
        
        //Eventos
        public const string VALIDAR_REFERENCIA = "validarReferencia";
        public const string AUTORIZAR_MONTO = "autorizarMonto";
        public const string NOTIFICAR_PAGO = "notificarPago";
    



    }
}
