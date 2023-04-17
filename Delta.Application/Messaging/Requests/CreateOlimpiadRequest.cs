using Delta.Application.Entities;

namespace Delta.Application.Messaging.Requests;

public record CreateOlimpiadRequest(string Name, string Url, OlimpiadType Type, DateTime Expires);