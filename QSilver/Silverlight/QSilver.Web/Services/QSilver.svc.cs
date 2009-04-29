using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;

namespace QSilver.Web.Services
{
    public class QSilver : DataService< QSilverEntities >
    {
        // Questo metodo viene chiamato solo una volta per inizializzare i criteri a livello di servizio.
        public static void InitializeService(IDataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
        }
    }
}
