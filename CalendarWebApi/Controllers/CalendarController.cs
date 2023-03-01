using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarWebApi.DataAccess;
using CalendarWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CalendarWebApi.DTO;

namespace CalendarWebApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CalendarController : ControllerBase
  {
    private readonly IRepository _repository;

    public CalendarController(IRepository repository)
    {
      _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> AddNewEvent([FromBody] CreateForm createForm)
    {
      Calendar calendar = new Calendar()
      {
        Name = createForm.Name,
        Location = createForm.Location,
        Time = createForm.Time,
        EventOrganizer = createForm.EventOrganizer,
        Members = createForm.Members
      };

      var result = await _repository.AddEvent(calendar);

      return new ObjectResult(result) { StatusCode = 201 };
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent([FromRoute] int id)
    {
      List<Calendar> eventToDeleteList = await _repository.GetCalendar(new EventQueryModel() { Id = id });
      if (eventToDeleteList.Count == 0) return NotFound();
      Calendar eventToDelete = eventToDeleteList[0];
      await _repository.DeleteEvent(eventToDelete);
      return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent([FromRoute] int id, [FromBody] CreateForm createForm)
    {
      List<Calendar> eventToUpdateList = await _repository.GetCalendar(new EventQueryModel() { Id = id });
      if (eventToUpdateList.Count == 0) return NotFound();
      Calendar calendarToUpdate = eventToUpdateList[0];
      calendarToUpdate.Time = createForm.Time;
      calendarToUpdate.Name = createForm.Name;
      calendarToUpdate.Location = createForm.Location;
      calendarToUpdate.Members = createForm.Members;
      calendarToUpdate.EventOrganizer = createForm.EventOrganizer;
      await _repository.UpdateEvent(calendarToUpdate);
      return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEventsQuery()
    {
      List<Calendar> allEvents = await _repository.GetCalendar();
      return Ok(allEvents);
    }

    // [HttpGet("query")]
    // public async Task<IActionResult> GetAllEventsQuery(Query query)
    // {
    //   EventQueryModel eventQueryModel = new EventQueryModel()
    //   {
    //     Id = query.id,
    //     EventOrganizer = query.eventOrganizer,
    //     Location = query.location,
    //     Name = query.name
    //   };
    //   List<Calendar> allEvents = await _repository.GetCalendar(eventQueryModel);
    //   return Ok(allEvents);
    // }

    [HttpGet("query")]
    public async Task<IActionResult> GetAllEventsQueryId(int id, string name, string eventOrganizer, string location)
    {
      List<Calendar> allEvents = await _repository.GetCalendar(new EventQueryModel() 
      { 
        Id = id,
        Name = name,
        EventOrganizer = eventOrganizer,
        Location = location
      });
      return Ok(allEvents);
    }
    /*

    [HttpGet]
    public async Task<IActionResult> GetAllEvents([FromQuery(Name = "query")] Query query)
    {
      EventQueryModel eventQueryModel = new EventQueryModel()
      {
        Id = query.id,
        EventOrganizer = query.eventOrganizer,
        Location = query.location,
        Name = query.name
      };
      List<Calendar> allEvents = await _repository.GetCalendar(eventQueryModel);
      if (allEvents.Count == 1)
      {
        Calendar calendarEvent = allEvents[0];
        return Ok(calendarEvent);
      }
      return Ok(allEvents);
    }

    */

    [HttpGet("sort")]
    public async Task<IActionResult> SortEvents()
    {
      List<Calendar> allEvents = await _repository.GetCalendar();
      allEvents.Sort(Calendar.CompareByTime);
      allEvents.Reverse();

      return Ok(allEvents);
    }
  }
}
