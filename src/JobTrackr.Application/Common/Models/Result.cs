namespace JobTrackr.Application.Common.Models;

public record Result<T>(bool IsSuccess, T? Data, string Error);