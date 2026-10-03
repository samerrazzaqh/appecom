using AppProject.API.Helper;
using AutoMapper;
using Core.DTO.AdministrationDTO.Roles;
using Core.DTO.AdministrationDTO.Users;
using Core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.API.Controllers;

[Authorize]
public class AdministrationController : BaseController
{
    public AdministrationController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
    {
    }

    [HttpGet("GetListRoles")]
    public async Task<IActionResult> GetListRoles()
    {
        try
        {
            var listRoles = await work.Administration.ListRoles();
            if (listRoles is null)
                return BadRequest(new ResponseAPI(400));
            return Ok(listRoles);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole(RolesDTO rolesDto)
    {
        try
        {
            var result = await work.Administration.CreateRole(rolesDto);
            if (result.success)
            {
                return Ok(new ResponseAPI(200, result.message));
            }
            else
            {
                return BadRequest(new ResponseAPI(400, result.message));
            }
            
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("DeleteRole/{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        try
        {
            var result = await work.Administration.DeleteRole(roleId);
            if (result.success)
            {
                return Ok(new ResponseAPI(200, result.message));
            }
            else
            {
                return BadRequest(new ResponseAPI(400, result.message));
            }
            
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("EditRole")]
    public async Task<IActionResult> EditRole(RolesEditDTO rolesEditDto)
    {
        try
        {
            var result = await work.Administration.EditRole(rolesEditDto);
            if (result.success)
            {
                return Ok(new ResponseAPI(200, result.message));
            }
            else
            {
                return BadRequest(new ResponseAPI(400, result.message));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("GetUsersInRole")]
    public async Task<IActionResult> GetUsersInRole(RolesDTO rolesDto)
    {
        try
        {
            var result = await work.Administration.GetUsersInRole(rolesDto);
            if (result.success)
            {
                return Ok(new ResponseAPI<RolesResponseDTO>(200, result, result.message));
            }
            else
            {
                return BadRequest(new ResponseAPI(400, result.message));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpPost("GetListUsers")]
    public async Task<IActionResult> GetListUsers([FromBody] UserDTO searchDto)
    {
        try
        {
            var result = await work.Administration.ListUsers(searchDto);
        
            if (result is null)
                return BadRequest(new ResponseAPI(400));

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpGet("GetUserClaimsAndRolesAsync/{userId}")]
    public async Task<IActionResult> GetUserClaimsAndRolesAsync(string userId)
    {
        try
        {
            var result = await work.Administration.GetUserClaimsAndRolesAsync(userId);
            if (result.success)
            {
                return Ok(new ResponseAPI<ClaimsRolesUser>(200, result, result.message));
            }
            else
            {
                return BadRequest(new ResponseAPI(400, result.message));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpPut("UpdateUserClaimsAndRoles")]
    public async Task<IActionResult> UpdateUserClaimsAndRoles([FromBody] UpdateClaimsRolesUserDTO dto)
    {
        try
        {
            var result = await work.Administration.UpdateUserClaimsAndRolesAsync(dto);

            if (result.success)
            {
                return Ok(new ResponseAPI<ClaimsRolesUser>(200, result, result.message));
            }

            return BadRequest(new ResponseAPI(400, result.message));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseAPI(500, ex.Message));
        }
    }
    
    
    
}