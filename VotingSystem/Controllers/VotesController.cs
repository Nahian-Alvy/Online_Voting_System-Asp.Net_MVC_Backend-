using System;
using System.Collections.Generic;
using System.Web.Http;
using VotingSystem.Business.DTOs;
using VotingSystem.Business.Services;

namespace VotingSystem.API.Controllers
{
    [RoutePrefix("api/votes")]
    public class VotesController : ApiController
    {
        private readonly IVotingService _votingService;

        public VotesController(IVotingService votingService)
        {
            _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var votes = _votingService.GetAllVotes() ?? new List<VoteDto>();
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("election/{electionId:int}")]
        public IHttpActionResult GetByElection(int electionId)
        {
            try
            {
                var votes = _votingService.GetVotesByElection(electionId) ?? new List<VoteDto>();
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("user/{userId:int}")]
        public IHttpActionResult GetByUser(int userId)
        {
            try
            {
                var votes = _votingService.GetVotesByUser(userId) ?? new List<VoteDto>();
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("candidate/{candidateId:int}")]
        public IHttpActionResult GetByCandidate(int candidateId)
        {
            try
            {
                var votes = _votingService.GetVotesByCandidate(candidateId) ?? new List<VoteDto>();
                return Ok(votes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CastVote(VoteCreateDto voteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (voteDto == null)
                    return BadRequest("Vote data is required");

                var vote = _votingService.CastVote(voteDto);
                return CreatedAtRoute("DefaultApi", new { id = vote.VoteId }, new { success = true, voteId = vote.VoteId });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("results")]
        public IHttpActionResult GetResults(int electionId)
        {
            try
            {
                var results = _votingService.GetElectionResults(electionId);
                if (results == null)
                    return NotFound();

                return Ok(results);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("has-voted")]
        public IHttpActionResult HasUserVoted(int userId, int electionId)
        {
            try
            {
                bool hasVoted = _votingService.HasUserVotedInElection(userId, electionId);
                return Ok(new { hasVoted = hasVoted });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}