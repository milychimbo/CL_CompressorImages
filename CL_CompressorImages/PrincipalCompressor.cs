using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.IO;

namespace CL_CompressorImages
{
    public class PrincipalCompressor : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            OrganizationServiceContext orgContext = new OrganizationServiceContext(service);

            try
            {
                if (context.MessageName.Equals("bit_compresorimagenes") && context.Stage.Equals(30))
                // Verificar si hay datos en los parámetros de entrada
                //if (context.InputParameters.Contains("image") && context.InputParameters["image"] is string image)
                {
                    string image = (string)context.InputParameters["image"];


                    tracingService.Trace("ingreso: {0}", image);
                    // Convertir de Base64 a Bytes
                    byte[] originalImageBytes = Convert.FromBase64String(image);
                    int originalSize = originalImageBytes.Length;

                    // Comprimir la imagen
                    byte[] compressedImageBytes = CompressImage(originalImageBytes, 75L); // Calidad al 75%

                    int compressedSize = compressedImageBytes.Length;

                    // Guardar el tamaño original y comprimido en las variables compartidas
                    context.SharedVariables["OriginalSize"] = originalSize;
                    context.SharedVariables["CompressedSize"] = compressedSize;

                    // Convertir de nuevo los bytes comprimidos a Base64
                    image = Convert.ToBase64String(compressedImageBytes);
                    tracingService.Trace("salida: {0}", image);
                   
                    context.OutputParameters["image"] = image;

                }
            }
            catch(Exception ex)
            {
                tracingService.Trace("bit_compresorimagenes: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred in bit_compresorimagenes. " + ex.Message, ex);
            }
            
        }

        private byte[] CompressImage(byte[] imageBytes, long quality)
        {
            // Validar la calidad (0-100)
            if (quality < 1 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 1 and 100.");

            // Usar ImageProcessor para comprimir la imagen
            using (var inStream = new MemoryStream(imageBytes))
            using (var outStream = new MemoryStream())
            {
                // Crear instancia del procesador de imágenes
                using (var imageFactory = new ImageProcessor.ImageFactory())
                {
                    // Cargar la imagen, ajustar la calidad y guardar en el stream de salida
                    imageFactory.Load(inStream)
                                .Quality((int)quality) // Establecer la calidad
                                .Save(outStream);
                }

                return outStream.ToArray();
            }
        }


    }
}