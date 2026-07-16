namespace WebBackend.Dto;
public record LanguageDto(
    string Language,
    IFormFile Startfile,
    IFormFile Testfile
);