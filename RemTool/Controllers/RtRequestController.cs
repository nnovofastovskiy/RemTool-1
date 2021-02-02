﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

using RemTool.Models;
using RemTool.Infrastructure.Interfaces.Services;
using RemTool.Infrastructure.Additional;
using Microsoft.AspNetCore.Authorization;
using RemTool.Services.Additional;

namespace RemTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RtRequestController : ControllerBase
    {
        private readonly IRtRequestService db;
        private readonly IRtMailSettingsService mailSettings;
        private readonly RtMailMessageService mailSender;

        public RtRequestController(IRtRequestService context, IRtMailSettingsService mailSettingsContext, RtMailMessageService mailSenderContext)
        {
            db = context;
            mailSettings = mailSettingsContext;
            mailSender = mailSenderContext;
        }

        // Get - get all requests
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<RtRequest>> Get()
        {
            return await db.ReadAllRtRequestsAsync();
        }

        // Get{id} - get request by id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<RtRequest> Get(string id)
        {
            RtRequest rtreq = await db.ReadRtRequestAsync(id);
            return rtreq;
        }

        // Post - add request
        [HttpPost]
        public async Task<IActionResult> Post(RtRequest newRtreq)
        {
            RtMailSettings mSettings = await mailSettings.ReadRtMailSettingsAsync();

            if (ModelState.IsValid)
            {
                var rtReq = await db.ReadRtRequestByEMailAsync(newRtreq.Email);

                if(rtReq != null)
                {
                    // < script >
                    //      var nowDate = new Date();
                    //      var day = nowDate.getDate();
                    //      var mounth = nowDate.getMonth();
                    //      var year = nowDate.getFullYear();
                    //      var hours = nowDate.getHours();
                    //      var minutes = nowDate.getMinutes();
                    //      var time = day + " " + mounth + " " + year + " " + hours + " " + minutes;
                    //      document.write(time);
                    // </ script >


                    if (newRtreq.SendedTime != null && rtReq.SendedTime != null)
                    {
                        var newTimeValues = newRtreq.SendedTime.Split(' ');
                        var oldTimeValues = rtReq.SendedTime.Split(' ');


                        if (newTimeValues.Length == 5 && oldTimeValues.Length == 5)
                        {
                            var newTime = 24 * int.Parse(newTimeValues[3]) + 60 * int.Parse(newTimeValues[4]);
                            var oldTime = 24 * int.Parse(oldTimeValues[3]) + 60 * int.Parse(oldTimeValues[4]);

                            if (newTime > oldTime + 3)
                            {
                                //await db.UpdateRtRequestAsync(new RtRequest
                                //{
                                //    Id = rtReq.Id,
                                //    Name = newRtreq.Name,
                                //    Email = newRtreq.Email,
                                //    Phone = newRtreq.Phone,
                                //    ReqInfo = newRtreq.ReqInfo,
                                //    SendedTime = newRtreq.SendedTime
                                //});
                                db.CreateRtRequest(newRtreq);

                                if (mSettings.SendNotificationToClient == true)
                                {
                                    mailSender.SendEMailMessageToClient(
                                        newRtreq.Email,
                                        mSettings.DefaultMessageToClient,
                                        mSettings.Credentials_Name,
                                        mSettings.Credentials_Pass,
                                        mSettings.SmtpServer_Host,
                                        mSettings.SmtpServer_Port);
                                }
                                if (mSettings.SendNotificationToHQ == true)
                                {
                                    mailSender.SendEMailMessageToHQ(
                                        mSettings.HQeMail,
                                        newRtreq,
                                        mSettings.Credentials_Name,
                                        mSettings.Credentials_Pass,
                                        mSettings.SmtpServer_Host,
                                        mSettings.SmtpServer_Port);
                                }
                                return Ok();
                            }
                            else
                            {
                                return BadRequest("wait");
                                
                            }
                        }
                    }
                    else
                    {
                       return BadRequest("bad time");
                    }
                }
                else
                {
                    db.CreateRtRequest(newRtreq);

                    if (mSettings.SendNotificationToClient == true)
                    {
                        mailSender.SendEMailMessageToClient(
                            newRtreq.Email,
                            mSettings.DefaultMessageToClient,
                            mSettings.Credentials_Name,
                            mSettings.Credentials_Pass,
                            mSettings.SmtpServer_Host,
                            mSettings.SmtpServer_Port);
                    }
                    if (mSettings.SendNotificationToHQ == true)
                    {
                        mailSender.SendEMailMessageToHQ(
                            mSettings.HQeMail,
                            newRtreq,
                            mSettings.Credentials_Name,
                            mSettings.Credentials_Pass,
                            mSettings.SmtpServer_Host,
                            mSettings.SmtpServer_Port);
                    }

                    return Ok(newRtreq);
                }
            }
            return BadRequest(ModelState);
        }

        // Put - update rtrequest (mark)
        [HttpPut]
        [Authorize]
        public IActionResult Put(RtRequest updatedRtreq)
        {
            if (ModelState.IsValid)
            {
                db.UpdateRtRequest(updatedRtreq);
                return Ok();
            }
            return BadRequest(ModelState);
        }


        // Delete{id} - delete request
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(string id)
        {
            RtRequest rtreq = db.ReadRtRequest(id);
            if (rtreq != null)
            {
                db.DeleteRtRequest(id);
            }
            return Ok(rtreq);
        }
    }
}
