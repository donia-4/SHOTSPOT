using SHOTSPOT.Domain.Common;

namespace SHOTSPOT.Domain.Sites;
public class LandingSite : AuditableEntity
{
    public string Name { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public string Description { get; private set; }

    public string DataSource { get; private set; }

    public LandingSite(
        string name,
        double latitude,
        double longitude,
        string description,
        string dataSource)
    {
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Description = description;
        DataSource = dataSource;
    }
}
