using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.IO;
using System.Linq;
using KFC.Entities;
using Newtonsoft.Json;

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

            /*KFCContext newContext = new KFCContext(service);

            string id = (string)context.InputParameters["bit_entradacompresormerged"];
            var evaluacion = newContext.bit_EvaluacionSet.Where(x => x.Id == Guid.Parse(id)).FirstOrDefault();

            //transformar a json
            var json = JsonConvert.SerializeObject(evaluacion);

            context.OutputParameters["bit_salidacompresormerged"] = json;
            */
            /*
            try
            {
                if (context.MessageName.Equals("bit_compresormerged") && context.Stage.Equals(30))
                {
                    string image = (string)context.InputParameters["bit_entradacompresormerged"];

                    tracingService.Trace("Ingreso: {0}", image);

                    // Convertir de Base64 a Bytes
                    byte[] originalImageBytes = Convert.FromBase64String(image);
                    int originalSize = originalImageBytes.Length;

                    // Comprimir la imagen
                    byte[] compressedImageBytes = CompressImage(originalImageBytes, 75); // Calidad al 75%

                    int compressedSize = compressedImageBytes.Length;

                    // Guardar el tamaño original y comprimido en las variables compartidas
                    context.SharedVariables["OriginalSize"] = originalSize;
                    context.SharedVariables["CompressedSize"] = compressedSize;

                    // Convertir de nuevo los bytes comprimidos a Base64
                    image = Convert.ToBase64String(compressedImageBytes);
                    tracingService.Trace("Salida: {0}", image);

                    context.OutputParameters["bit_salidacompresormerged"] = image;
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error en bit_compresorimagenes: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Ocurrió un error en bit_compresorimagenes. " + ex.Message, ex);
            }*/
        }

        private byte[] CompressImage(byte[] imageBytes, int quality)
        {
            if (quality < 1 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality), "La calidad debe estar entre 1 y 100.");

            using (var processor = new ImageFactory(preserveExifData: false))
            {
                using (var stream = new MemoryStream(imageBytes))
                {
                    // Cargar la imagen
                    processor.Load(stream);

                    // Redimensionar la imagen si es demasiado grande
                    int maxDimension = 1024; // Limitar el tamaño máximo de la imagen a 1024px
                    if (processor.Image.Width > maxDimension || processor.Image.Height > maxDimension)
                    {
                        processor.Resize(new System.Drawing.Size(maxDimension, maxDimension));
                    }

                    // Ajustar la calidad de compresión
                    processor.Format(new JpegFormat { Quality = quality });

                    // Guardar la imagen comprimida a un nuevo stream
                    using (var outputStream = new MemoryStream())
                    {
                        processor.Save(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
        }
    }
}
