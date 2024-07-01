using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;
using NZWalksAPI.Validations;
using System.Collections.Generic;
using System.Text.Json;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext _dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IMapper mapper, ILogger<RegionsController> logger)
        {
            _dbContext = dbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles ="Reader,Writer")]
        public IActionResult GetAllRegions()
        {
            logger.LogWarning("Getting all regions");
            var regions = _dbContext.Regions.Select(x => new RegionDTO { Code = x.Code, Id = x.Id, Name = x.Name, RegionImageURL = x.RegionImageURL }).ToList();

            logger.LogInformation(message: JsonSerializer.Serialize(regions));
            return Ok(regions);

        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]
        public IActionResult GetRegion([FromRoute] Guid id)
        {

            var regions = _dbContext.Regions
                .Select(x => new RegionDTO { Code = x.Code, Id = x.Id, Name = x.Name, RegionImageURL = x.RegionImageURL })
                .FirstOrDefault(x => x.Id.Equals(id));
            if (regions == null) { return NotFound(); }
            return Ok(regions);

        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public IActionResult Create([FromBody] AddRegionDTO addRegionDTO)
        {
            if (addRegionDTO == null) { return NotFound(); }
            Region region = new()
            {
                Code = addRegionDTO.Code,
                Name = addRegionDTO.Name,
                RegionImageURL = addRegionDTO.RegionImageURL
            };

            _dbContext.Regions.Add(region);
            _dbContext.SaveChanges();

            // RegionDTO regionDTO = new() { Id = region.Id, Code = region.Code, Name = region.Name, RegionImageURL = region.RegionImageURL };
            RegionDTO regionDTO = mapper.Map<RegionDTO>(region);

            return CreatedAtAction(nameof(GetRegion), new { id = regionDTO.Id }, regionDTO);

        }

        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public IActionResult Update([FromRoute] Guid id, UpdateRegionDTO updateRegionDTO)
        {
            var region = _dbContext.Regions.FirstOrDefault(x => x.Id.Equals(id));
            if (region == null) { return NotFound(); }
            region.Code = updateRegionDTO.Code;
            region.Name = updateRegionDTO.Name;
            region.RegionImageURL = updateRegionDTO.RegionImageURL;

            _dbContext.SaveChanges();

            RegionDTO regionDto = new()
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageURL = region.RegionImageURL
            };

            return Ok(regionDto);
        }
    }
}
