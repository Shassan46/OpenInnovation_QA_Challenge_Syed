using OpenInnovation_QA_Challenge.Models;
using RestSharp;
using System.Net;

namespace OpenInnovation_QA_Challenge
{
    [TestFixture]
    public class APITests
    {
        private RestClient _client;
        private string? _modelId;
        private string? _versionId;

        [SetUp]
        public void Setup()
        {
            _client = new RestClient("http://localhost:8000");
        }
       
        [Test, Order(1)]
        public async Task Models_POST_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest("models");
            var modelAdd = new { Name = "My Model123", Owner = "john123" };

            request.AddJsonBody(modelAdd);

            //Act
            var response = await _client.ExecutePostAsync<Model>(request);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Data, Is.Not.Null, $"response data {response.Data} is null");

                Assert.That(response.Data.Name, Is.EqualTo("My Model123"));
                Assert.That(response.Data.Owner, Is.EqualTo("john123"));

                _modelId = response.Data.Id;

                Assert.That(_modelId, Is.Not.Null.And.Not.Empty, "Model ID should not be null or empty.");
            });
            
        }

        [Test, Order(2)]
        public async Task Models_GET_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest("/models");

            //Act
            var response = await _client.ExecuteGetAsync(request);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });
        }

        [Test, Order(3)]
        public async Task Versions_POST_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest($"models/{_modelId}/versions");
            var versionAdd = new
            {
                name = "Version 1 - Tiny Llama",
                hugging_face_model = "TinyLlama/TinyLlama-1.1B-Chat-v1.0"
            };

            request.AddJsonBody(versionAdd);

            //Act
            var response = await _client.ExecutePostAsync<ModelVersion>(request);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Data, Is.Not.Null);

                Assert.That(response.Data.Name, Is.EqualTo("Version 1 - Tiny Llama"));

                _versionId = response.Data.Id;

                Assert.That(_versionId, Is.Not.Null.And.Not.Empty, "Version ID should not be null or empty.");
            });

            
        }

        [Test, Order(4)]
        public async Task Versions_GET_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest($"/models/{_modelId}/versions");

            //Act
            var response = await _client.ExecuteGetAsync(request);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });
        }

        [Test, Order(5)]
        public async Task Inference_POST_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest($"/models/{_modelId}/versions/{_versionId}/infer", Method.Post);
            var inferenceArgs = new InferenceArguments { Text = "Hi, how are you?" };
            request.AddJsonBody(inferenceArgs);

            //Act
            var response = await _client.ExecuteAsync(request);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty);
            });
        }

        [Test, Order(6)]
        public async Task Model_POST_ShouldFail_WhenOwnerFieldIsEmpty()
        {
            //Arrange
            var request = new RestRequest("/models");
            var modelAdd = new Model { Name = "My Model", Owner = "" }; 
            request.AddJsonBody(modelAdd);

            //Act
            var response = await _client.ExecutePostAsync(request);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity), $"The API incorrectly accepted a model with empty owner.  {response.Content}");
        }

        [Test, Order(7)]
        public async Task Versions_DELETE_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest($"/models/{_modelId}/versions/{_versionId}");

            //Act
            var response = await _client.ExecuteDeleteAsync(request);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
        }

        [Test, Order(8)]
        public async Task Model_Delete_ShouldReturnSuccess()
        {
            //Arrange
            var request = new RestRequest($"/models/{_modelId}");

            //Act
            var response = await _client.ExecuteDeleteAsync(request);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), response.ErrorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }
    }

}
