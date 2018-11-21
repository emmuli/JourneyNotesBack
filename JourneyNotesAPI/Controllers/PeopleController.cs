﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JourneyEntities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace JourneyNotesAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DocumentClient _client;
        private const string _dbName = "JourneyNotesDB";
        private const string _collectionNamePerson = "Person";
        private const string _collectionNameTrip = "Trip";
        private const string _collectionNamePitstop = "Pitstop";

        public PeopleController(IConfiguration configuration)
        {
            _configuration = configuration;

            var endpointUri =
            _configuration["ConnectionStrings:CosmosDbConnection:EndpointUri"];

            var key =
            _configuration["ConnectionStrings:CosmosDbConnection:PrimaryKey"];

            _client = new DocumentClient(new Uri(endpointUri), key);
        }

        // GET: api/Person
        //[HttpGet]
        //public IEnumerable<string> GetPersons()
        //{
        //    return new string[] { "valueX", "valueY" };
        //}

        // GET: api/people/5
        [HttpGet("{id}", Name = "GetPerson")]
        public ActionResult<string> GetPerson(int id)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
            IQueryable<Person> query = _client.CreateDocumentQuery<Person>(
            UriFactory.CreateDocumentCollectionUri(_dbName, _collectionNamePerson),
            $"SELECT * FROM C WHERE C.PersonId = {id}", queryOptions);
            var person = query.ToList().FirstOrDefault();

            return Ok(person);
        }

        // POST: api/people
        [HttpPost]
        public async Task<ActionResult<string>> PostPerson([FromBody] Person person)
        {
            Document document = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_dbName, _collectionNamePerson), person);
            return Ok(document.Id);
        }

        // PUT: api/Person/5
        //[HttpPut("{id}")]
        //public void PutPerson(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void DeletePerson(int id)
        //{
        //}
    }
}
