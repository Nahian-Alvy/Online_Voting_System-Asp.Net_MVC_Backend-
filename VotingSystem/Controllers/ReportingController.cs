using System;
using System.Web.Http;
using VotingSystem.Business.Services;

namespace VotingSystem.API.Controllers
{
    [RoutePrefix("api/reporting")]
    public class ReportingController : ApiController
    {
        private readonly IReportingService _reportingService;

        public ReportingController(IReportingService reportingService)
        {
            _reportingService = reportingService ?? throw new ArgumentNullException(nameof(reportingService));
        }

        [HttpGet]
        [Route("system-statistics")]
        public IHttpActionResult GetSystemStatistics()
        {
            try
            {
                // Current date is 2025-06-25 05:52:58
                var currentDate = DateTime.Parse("2025-06-25 05:52:58");
                var statistics = _reportingService.GetSystemStatistics(currentDate);

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("voting-trends")]
        public IHttpActionResult GetVotingTrends(int electionId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Default to last 7 days if dates not provided
                var currentDate = DateTime.Parse("2025-06-25 05:52:58");
                startDate = startDate ?? currentDate.AddDays(-7);
                endDate = endDate ?? currentDate;

                var trends = _reportingService.GetVotingTrends(electionId, startDate.Value, endDate.Value);

                return Ok(trends);
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
        [Route("voter-demographics")]
        public IHttpActionResult GetVoterDemographics(int electionId)
        {
            try
            {
                var demographics = _reportingService.GetVoterDemographics(electionId);

                return Ok(demographics);
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
        [Route("candidate-performance")]
        public IHttpActionResult GetCandidatePerformance(int candidateId)
        {
            try
            {
                var performance = _reportingService.GetCandidatePerformance(candidateId);

                return Ok(performance);
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
        [Route("audit-trail")]
        public IHttpActionResult GetAuditTrail(int electionId)
        {
            try
            {
                var auditTrail = _reportingService.GetElectionAuditTrail(electionId);

                return Ok(auditTrail);
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
    }
}