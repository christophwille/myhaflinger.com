namespace MyHaflinger.Common.Services
{
	public interface ITemplateRenderingService
	{
		string Render(string source, object data);
	}
}