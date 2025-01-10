using CL_CompressorImages;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;

namespace CL_CompressorImages.Tests
{
    public class PrincipalCompressorTests
    {
        [Fact]
        public void Plugin_CompressImage_Should_Work_And_Reduce_Size()
        {
            // Configurar el contexto simulado
            var context = new XrmFakedContext();

            // Crear la entidad simulada con datos de entrada
            var inputEntity = new Entity("custom_entity");
            byte[] originalImageBytes = new byte[1024 * 1024 * 10]; // Simula una imagen de 10 MB
            string imageBase64 = Convert.ToBase64String(originalImageBytes);
            inputEntity["image"] = imageBase64;

            // Crear el contexto de ejecución simulado
            var pluginContext = context.GetDefaultPluginContext();
            pluginContext.InputParameters["Target"] = inputEntity;

            // Crear el FakeServiceProvider
            var serviceProvider = new FakeServiceProvider(pluginContext);

            // Instanciar el plugin
            var plugin = new PrincipalCompressor();

            // Ejecutar el plugin usando el servicio simulado
            plugin.Execute(serviceProvider);

            // Obtener los tamaños de las variables compartidas
            int originalSize = (int)pluginContext.SharedVariables["OriginalSize"];
            int compressedSize = (int)pluginContext.SharedVariables["CompressedSize"];

            // Calcular la reducción en porcentaje
            double reduction = ((double)(originalSize - compressedSize) / originalSize) * 100;

            // Imprimir resultados en la consola
            Console.WriteLine($"Tamaño original: {originalSize / (1024 * 1024)} MB");
            Console.WriteLine($"Tamaño comprimido: {compressedSize / (1024 * 1024)} MB");
            Console.WriteLine($"Reducción: {reduction:F2}%");

            // Verificar que la compresión realmente redujo el tamaño
            Assert.True(compressedSize < originalSize, "La compresión no redujo el tamaño.");
        }
    }
}
