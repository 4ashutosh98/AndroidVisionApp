using Microsoft.EntityFrameworkCore;
using VisionService.Models;

namespace VisionService.Data;

/// <summary>
/// Entity Framework database context for participant management
/// Uses in-memory SQLite for fast, temporary storage during application runtime
/// </summary>
public class ParticipantDbContext : DbContext
{
    public ParticipantDbContext(DbContextOptions<ParticipantDbContext> options) : base(options)
    {
    }

    public DbSet<Participant> Participants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Participant entity
        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Create indexes for better search performance
            entity.HasIndex(e => e.FirstName);
            entity.HasIndex(e => e.LastName);
            entity.HasIndex(e => new { e.FirstName, e.LastName });
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Seed database with dummy data
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds the database with realistic dummy participant data
    /// </summary>
    private void SeedData(ModelBuilder modelBuilder)
    {
        var participants = new List<Participant>
        {
            // Tech Conference Participants
            new() { Id = 1, FirstName = "John", LastName = "Smith", CompanyName = "Microsoft", JobTitle = "Software Engineer", Email = "john.smith@microsoft.com" },
            new() { Id = 2, FirstName = "Sarah", LastName = "Johnson", CompanyName = "Google", JobTitle = "Product Manager", Email = "sarah.j@google.com" },
            new() { Id = 3, FirstName = "Michael", LastName = "Davis", CompanyName = "Amazon", JobTitle = "Senior Developer", Email = "m.davis@amazon.com" },
            new() { Id = 4, FirstName = "Emily", LastName = "Brown", CompanyName = "Meta", JobTitle = "UX Designer", Email = "emily.brown@meta.com" },
            new() { Id = 5, FirstName = "David", LastName = "Wilson", CompanyName = "Apple", JobTitle = "iOS Developer", Email = "d.wilson@apple.com" },
            
            // Startup Entrepreneurs
            new() { Id = 6, FirstName = "Jessica", LastName = "Martinez", CompanyName = "TechStart Inc", JobTitle = "CEO", Email = "jessica@techstart.com" },
            new() { Id = 7, FirstName = "Andrew", LastName = "Taylor", CompanyName = "InnovateLab", JobTitle = "CTO", Email = "andrew.t@innovatelab.io" },
            new() { Id = 8, FirstName = "Lisa", LastName = "Anderson", CompanyName = "DataFlow Systems", JobTitle = "Data Scientist", Email = "lisa.anderson@dataflow.com" },
            new() { Id = 9, FirstName = "Robert", LastName = "Thomas", CompanyName = "CloudNine", JobTitle = "DevOps Engineer", Email = "rob.thomas@cloudnine.net" },
            new() { Id = 10, FirstName = "Amanda", LastName = "Jackson", CompanyName = "AI Ventures", JobTitle = "ML Engineer", Email = "amanda@aiventures.com" },
            
            // Academic & Research
            new() { Id = 11, FirstName = "Dr. James", LastName = "White", CompanyName = "MIT", JobTitle = "Computer Science Professor", Email = "j.white@mit.edu" },
            new() { Id = 12, FirstName = "Prof. Maria", LastName = "Garcia", CompanyName = "Stanford University", JobTitle = "AI Researcher", Email = "maria.garcia@stanford.edu" },
            new() { Id = 13, FirstName = "Daniel", LastName = "Lee", CompanyName = "Carnegie Mellon", JobTitle = "PhD Student", Email = "daniel.lee@cmu.edu" },
            new() { Id = 14, FirstName = "Rachel", LastName = "Moore", CompanyName = "Berkeley Lab", JobTitle = "Research Scientist", Email = "r.moore@berkeley.edu" },
            new() { Id = 15, FirstName = "Christopher", LastName = "Clark", CompanyName = "Harvard", JobTitle = "Postdoc Researcher", Email = "c.clark@harvard.edu" },
            
            // Consulting & Finance
            new() { Id = 16, FirstName = "Jennifer", LastName = "Lewis", CompanyName = "McKinsey & Co", JobTitle = "Senior Consultant", Email = "jennifer.lewis@mckinsey.com" },
            new() { Id = 17, FirstName = "Mark", LastName = "Walker", CompanyName = "Goldman Sachs", JobTitle = "Quantitative Analyst", Email = "mark.walker@gs.com" },
            new() { Id = 18, FirstName = "Nicole", LastName = "Hall", CompanyName = "Deloitte", JobTitle = "Technology Consultant", Email = "nicole.hall@deloitte.com" },
            new() { Id = 19, FirstName = "Kevin", LastName = "Allen", CompanyName = "JPMorgan Chase", JobTitle = "VP Technology", Email = "kevin.allen@jpmorgan.com" },
            new() { Id = 20, FirstName = "Stephanie", LastName = "Young", CompanyName = "Accenture", JobTitle = "Digital Transformation Lead", Email = "s.young@accenture.com" },
            
            // International Participants
            new() { Id = 21, FirstName = "Hiroshi", LastName = "Tanaka", CompanyName = "Sony", JobTitle = "Robotics Engineer", Email = "h.tanaka@sony.jp" },
            new() { Id = 22, FirstName = "Priya", LastName = "Sharma", CompanyName = "Infosys", JobTitle = "Software Architect", Email = "priya.sharma@infosys.com" },
            new() { Id = 23, FirstName = "Lars", LastName = "Andersen", CompanyName = "Spotify", JobTitle = "Backend Developer", Email = "lars.andersen@spotify.com" },
            new() { Id = 24, FirstName = "Elena", LastName = "Popov", CompanyName = "Yandex", JobTitle = "Data Engineer", Email = "elena.popov@yandex.ru" },
            new() { Id = 25, FirstName = "Carlos", LastName = "Rodriguez", CompanyName = "Telefonica", JobTitle = "Network Engineer", Email = "carlos.rodriguez@telefonica.es" },
            
            // Healthcare & Biotech
            new() { Id = 26, FirstName = "Dr. Susan", LastName = "Chen", CompanyName = "Moderna", JobTitle = "Biotech Researcher", Email = "susan.chen@moderna.com" },
            new() { Id = 27, FirstName = "Thomas", LastName = "Kim", CompanyName = "Pfizer", JobTitle = "Clinical Data Analyst", Email = "thomas.kim@pfizer.com" },
            new() { Id = 28, FirstName = "Lauren", LastName = "Murphy", CompanyName = "Johnson & Johnson", JobTitle = "Digital Health Lead", Email = "lauren.murphy@jnj.com" },
            new() { Id = 29, FirstName = "Benjamin", LastName = "Scott", CompanyName = "Roche", JobTitle = "Bioinformatics Specialist", Email = "ben.scott@roche.com" },
            new() { Id = 30, FirstName = "Melissa", LastName = "Green", CompanyName = "Mayo Clinic", JobTitle = "Health Informatics Director", Email = "melissa.green@mayo.edu" },
            
            // Government & Non-Profit
            new() { Id = 31, FirstName = "William", LastName = "Baker", CompanyName = "NASA", JobTitle = "Systems Engineer", Email = "william.baker@nasa.gov" },
            new() { Id = 32, FirstName = "Samantha", LastName = "Adams", CompanyName = "CDC", JobTitle = "Epidemiologist", Email = "samantha.adams@cdc.gov" },
            new() { Id = 33, FirstName = "Richard", LastName = "Phillips", CompanyName = "Red Cross", JobTitle = "Technology Director", Email = "richard.phillips@redcross.org" },
            new() { Id = 34, FirstName = "Michelle", LastName = "Turner", CompanyName = "United Nations", JobTitle = "Data Analyst", Email = "michelle.turner@un.org" },
            new() { Id = 35, FirstName = "Patrick", LastName = "Campbell", CompanyName = "World Bank", JobTitle = "Digital Innovation Lead", Email = "patrick.campbell@worldbank.org" },
            
            // Media & Creative
            new() { Id = 36, FirstName = "Alexandra", LastName = "Parker", CompanyName = "Netflix", JobTitle = "Content Technology Lead", Email = "alex.parker@netflix.com" },
            new() { Id = 37, FirstName = "Ryan", LastName = "Evans", CompanyName = "Adobe", JobTitle = "Creative Cloud Engineer", Email = "ryan.evans@adobe.com" },
            new() { Id = 38, FirstName = "Catherine", LastName = "Roberts", CompanyName = "Disney", JobTitle = "Animation Technology", Email = "catherine.roberts@disney.com" },
            new() { Id = 39, FirstName = "Jonathan", LastName = "Collins", CompanyName = "Warner Bros", JobTitle = "VFX Supervisor", Email = "jonathan.collins@warnerbros.com" },
            new() { Id = 40, FirstName = "Natalie", LastName = "Stewart", CompanyName = "Pixar", JobTitle = "Rendering Engineer", Email = "natalie.stewart@pixar.com" },
            
            // Additional Diverse Names for Better Fuzzy Testing
            new() { Id = 41, FirstName = "Mohammed", LastName = "Al-Rashid", CompanyName = "Emirates Tech", JobTitle = "Cloud Architect", Email = "mohammed.rashid@emirates.ae" },
            new() { Id = 42, FirstName = "Anna-Maria", LastName = "Kowalski", CompanyName = "SAP", JobTitle = "Enterprise Consultant", Email = "anna.kowalski@sap.com" },
            new() { Id = 43, FirstName = "Jean-Pierre", LastName = "Dubois", CompanyName = "Orange", JobTitle = "Telecom Engineer", Email = "jp.dubois@orange.fr" },
            new() { Id = 44, FirstName = "Xiaowei", LastName = "Zhang", CompanyName = "Alibaba", JobTitle = "E-commerce Developer", Email = "xiaowei.zhang@alibaba.com" },
            new() { Id = 45, FirstName = "Isabella", LastName = "Rossi", CompanyName = "Ferrari", JobTitle = "Automotive Software", Email = "isabella.rossi@ferrari.it" }
        };

        modelBuilder.Entity<Participant>().HasData(participants);
    }
}
