using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Chat.Hubs;

namespace Chat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
       private readonly DatabaseContext _context;
    private readonly IHubContext<ChatHub> _hub;
    public MessagesController(DatabaseContext context, IHubContext<ChatHub> hub)
    {
        _context = context;
        _hub = hub;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
    {
        return await _context.Messages
            .OrderBy(c => c.Created)
            .ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var Message = await _context.Messages.FindAsync(id);

        if (Message == null)
        {
            return NotFound();
        }

        return Message;
    }

      [HttpPost]
    public async Task<ActionResult<Message>> PostMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Send a message to all clients listening to the hub
        await _hub.Clients.All.SendAsync("ReceiveMessage", message);

        return CreatedAtAction("GetMessage", new { id = message.Id }, message);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> PutMessage(int id, Message message)
    {
        if (id != message.Id)
        {
            return BadRequest();
        }

        _context.Entry(message).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var Message = await _context.Messages.FindAsync(id);
        if (Message == null)
        {
            return NotFound();
        }

        _context.Messages.Remove(Message);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}