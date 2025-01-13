using KFC.Entities;
using Microsoft.Identity.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CL_PDFMakerKFC
{
    public class CreatorPdf : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            OrganizationServiceContext orgContext = new OrganizationServiceContext(service);

            string url = "https://kfc-dev.crm.dynamics.com/";
            if (context.InputParameters.Contains("bit_entradacreatorpdf") && context.InputParameters["bit_entradacreatorpdf"] is string)
            {
                try
                {
                    //get token from auth headers
                    /*string token = string.Empty;
                    token = GetToken().GetAwaiter().GetResult();
                    */
                    KFCContext newContext = new KFCContext(service);

                    var evaluacion = newContext.bit_EvaluacionSet.Where(e => e.bit_EvaluacionId == new Guid(context.InputParameters["bit_entradacreatorpdf"].ToString())).FirstOrDefault();
                    var json = "OK";
                    if (evaluacion != null)
                    {
                       /* if (!string.IsNullOrEmpty(evaluacion.bit_BuenasPracticasImagen_URL))
                        {
                            string imageUrl = url + evaluacion.bit_BuenasPracticasImagen_URL;

                            // Descargar la imagen como Base64
                            string base64Image = GetImageAsBase64(imageUrl, token).GetAwaiter().GetResult();

                            byte[] imageBytes = Convert.FromBase64String(base64Image);

                            // Agregar la imagen como un campo en el JSON
                            evaluacion.bit_BuenasPracticasImagen = imageBytes;
                        }
                       */
                        // Serializar la evaluación como JSON
                         json = JsonSerializer.Serialize(evaluacion);
                    }
                        // Get the record
                        context.OutputParameters["bit_salidacreatorpdf"] = json;
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }


            }
        }
        
       

        private async Task<string> GetImageAsBase64(string imageUrl, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                // Agregar el token al encabezado de la solicitud
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Hacer la solicitud para obtener la imagen
                HttpResponseMessage response = await client.GetAsync(imageUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Leer la imagen como bytes
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    // Convertir a Base64
                    return Convert.ToBase64String(imageBytes);
                }
                else
                {
                    throw new Exception($"Error al descargar la imagen: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }
    }
}
