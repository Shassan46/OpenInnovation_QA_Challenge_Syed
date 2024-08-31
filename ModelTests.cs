using Newtonsoft.Json;
using NUnit.Framework;
using OpenInnovation_QA_Challenge.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace OpenInnovation_QA_Challenge
{
    [TestFixture]
    public class ModelTests
    {
        private RestClient _client;
        private string? _modelId;
        private string? _versionId;

        [SetUp]
        public void Setup()
        {
            _client = new RestClient("http://localhost:8002"); // Replace with actual API URL
        }

        // 1. Add Model and Save ID
        [Test, Order(1)]
        public async Task AddModel_ShouldReturnSuccessAndSaveId()
        {
            var request = new RestRequest("models");
            var modelAdd = new { Name = "My Model123", Owner = "john123" };
            request.AddJsonBody(modelAdd);

            var response = await _client.ExecutePostAsync<Model>(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Data, Is.Not.Null, $"response data {response.Data} is null");

                Assert.That(response.Data.Name, Is.EqualTo("My Model123"));
                Assert.That(response.Data.Owner, Is.EqualTo("john123"));

                // Save the model ID for later use
                _modelId = response.Data.Id;
                Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID should not be null or empty.");
            });
            
        }

        // 2. Get Models
        [Test, Order(2)]
        public async Task GetModels_ShouldReturnSuccess()
        {
            var request = new RestRequest("/models");

            var response = await _client.ExecuteGetAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);

                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });
        }

        // 3. Add Model Version and Save Version ID
        [Test, Order(3)]
        public async Task AddModelVersion_ShouldReturnSuccessAndSaveVersionId()
        {
            // Ensure _modelId is not null
            Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID is null or empty. Ensure that AddModel test runs first.");

            var request = new RestRequest($"models/{_modelId}/versions");
            var versionAdd = new             {
                name = "Version 1 - Tiny Llama",
                hugging_face_model = "TinyLlama/TinyLlama-1.1B-Chat-v1.0"
            };
            request.AddJsonBody(versionAdd);

            var response = await _client.ExecutePostAsync<ModelVersion>(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Data, Is.Not.Null);

                Assert.That(response.Data.Name, Is.EqualTo("Version 1 - Tiny Llama"));

                // Save the version ID for later use
                _versionId = response.Data.Id;
                Assert.That(_versionId, Is.Not.Null.And.Not.Empty, "Version ID should not be null or empty.");
            });

            
        }

        // 4. Get Model Versions Using Saved Model ID
        [Test, Order(4)]
        public async Task GetModelVersions_ShouldReturnSuccessUsingSavedId()
        {
            // Ensure _modelId is not null
            Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID is null or empty. Ensure that AddModel test runs first.");

            var request = new RestRequest($"/models/{_modelId}/versions");

            var response = await _client.ExecuteGetAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });
        }

        // 5. Perform Inference Using Saved IDs
        [Test, Order(5)]
        public async Task PerformInference_ShouldReturnSuccessUsingSavedIds()
        {
            Assert.Multiple(() =>
            {
                // Ensure _modelId and _versionId are not null
                Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID is null or empty. Ensure that AddModel test runs first.");
                Assert.That(_versionId, Is.Not.Null.And.Not.Empty, "Version ID is null or empty. Ensure that AddModelVersion test runs first.");
            });

            var request = new RestRequest($"/models/{_modelId}/versions/{_versionId}/infer", Method.Post);
            var inferenceArgs = new InferenceArguments { Text = "Hi, how are you?" };
            request.AddJsonBody(inferenceArgs);

            var response = await _client.ExecuteAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });
        }

        // 6. Negative Test: Add Model with Missing Owner
        [Test, Order(6)]
        public async Task AddModel_ShouldFail_WhenOwnerIsMissing()
        {
            var request = new RestRequest("/models");
            
            // Missing owner
            var modelAdd = new ModelAdd { Name = "My Model", Owner = "" }; 
            request.AddJsonBody(modelAdd);

            var response = await _client.ExecutePostAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity), $"The API incorrectly accepted a model with empty owner.  {response.Content}");
        }

        // 7. Delete Model Version Using Saved Version ID
        [Test, Order(7)]
        public async Task DeleteModelVersion_ShouldReturnSuccessUsingSavedIds()
        {
            Assert.Multiple(() =>
            {
                // Ensure _modelId and _versionId are not null
                Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID is null or empty. Ensure that AddModel test runs first.");
                Assert.That(_versionId, Is.Not.Null.And.Not.Empty, "Version ID is null or empty. Ensure that AddModelVersion test runs first.");
            });

            var request = new RestRequest($"/models/{_modelId}/versions/{_versionId}");

            var response = await _client.ExecuteDeleteAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
        }

        // 8. Delete Model Using Saved ID
        [Test, Order(8)]
        public async Task DeleteModel_ShouldReturnSuccessUsingSavedId()
        {
            // Ensure _modelId is not null
            Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID is null or empty. Ensure that AddModel test runs first.");

            var request = new RestRequest($"/models/{_modelId}");

            var response = await _client.ExecuteDeleteAsync(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }
    }

}
