using AkakceProject.Application.Mediators;
using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AkakceProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CampaignController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetCampaigns")]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var campaigns = await _mediator.Send(new CampaignMediator.GetAllCampaignsQuery());
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaigns query: {stopwatch.Elapsed}");
                return Ok(campaigns.Take(200));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaigns query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCampaignById")]
        public async Task<ActionResult<Campaign>> GetCampaign(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var campaign = await _mediator.Send(new CampaignMediator.GetCampaignQuery { CampaignId = id });
                if (campaign == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for GetCampaign query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaign query: {stopwatch.Elapsed}");
                return Ok(campaign);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaign query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCampaignsByUserId")]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetUserCampaigns(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var campaigns = await _mediator.Send(new CampaignMediator.GetUserCampaignsQuery { UserId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUserCampaigns query: {stopwatch.Elapsed}");
                return Ok(campaigns.Take(200));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUserCampaigns query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCampaignInfo")]
        public async Task<ActionResult<CampaignResponse>> GetCampaignInfo(int userId, int campaignId)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var campaign = await _mediator.Send(new CampaignMediator.GetCampaignInfoQuery { UserId = userId, CampaignId = campaignId });
                if (campaign == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for GetCampaignInfo query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaignInfo query: {stopwatch.Elapsed}");
                return Ok(campaign);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaignInfo query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateCampaign")]
        public async Task<ActionResult<Campaign>> CreateCampaign(CampaignForCreationDto campaign)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var createdCampaign = await _mediator.Send(new CampaignMediator.CreateCampaignQuery { Campaign = campaign });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for CreateCampaign query: {stopwatch.Elapsed}");
                return CreatedAtAction(nameof(GetCampaign), new { id = createdCampaign.CampaignId }, createdCampaign);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for CreateCampaign query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateCampaign")]
        public async Task<ActionResult> UpdateCampaign(int id, CampaignForUpdateDto campaign)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var dbCampaign = await _mediator.Send(new CampaignMediator.GetCampaignQuery { CampaignId = id });
                if (dbCampaign == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for UpdateCampaign query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                await _mediator.Send(new CampaignMediator.UpdateCampaignQuery { CampaignId = id, Campaign = campaign });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for UpdateCampaign query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for UpdateCampaign query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCampaign")]
        public async Task<ActionResult> DeleteCampaign(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var dbCampaign = await _mediator.Send(new CampaignMediator.GetCampaignQuery { CampaignId = id });
                if (dbCampaign == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for DeleteCampaign query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                await _mediator.Send(new CampaignMediator.DeleteCampaignQuery { CampaignId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteCampaign query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteCampaign query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCampaignsByUserId")]
        public async Task<ActionResult> DeleteUserCampaigns(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await _mediator.Send(new CampaignMediator.DeleteUserCampaignsQuery { UserId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteUserCampaigns query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteUserCampaigns query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}