using System;
using System.Threading.Tasks;
using cosmoschat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace cosmoschat.Controllers
{
    public class PostsController : Controller
    {
        private readonly IDocumentClient _documentClient;
        private readonly Uri _collectionLink = UriFactory.CreateDocumentCollectionUri("social", "posts");
        public PostsController(IDocumentClient documentClient)
        {
            _documentClient = documentClient;
        }

        [HttpPost]
        [Route("posts/save")]
        public async Task<IActionResult> Save([FromBody] Models.MessagePayload value)
        {
            var doc = await _documentClient.CreateDocumentAsync(_collectionLink, value);
            return Ok(doc.Resource.Id);
        }

        [HttpGet]
        [Route("posts/get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var link = UriFactory.CreateDocumentUri("social", "results", id);

            try
            {
                var doc = await _documentClient.ReadDocumentAsync(link);
                var result = new Result()
                {
                    id = id,
                    score = doc.Resource.GetPropertyValue<double>("score")
                };
                return Ok(result);
            }
            catch(DocumentClientException ex)
            {
                if(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Text Analytics result is not ready yet
                    return Ok(null);
                }

                throw ex;
            }
        }
    }
}
