using NUnit.Framework;
using SportZone.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace SportZone.Tests.Models;

/// <summary>
/// Unit tests for the Event model (renamed from SportActivity)
/// Testing the renamed entity to ensure all properties and functionality work correctly
/// </summary>
[TestFixture]
public class EventModelTests
{
    [Test]
    public void Event_Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var sportEvent = new Event();

        // Assert
        Assert.That(sportEvent.UniqueId, Is.Not.Null);
        Assert.That(sportEvent.Name, Is.EqualTo(string.Empty));
        Assert.That(sportEvent.MinParticipants, Is.EqualTo(1));
        Assert.That(sportEvent.CurrentParticipants, Is.EqualTo(0));
        Assert.That(sportEvent.Participants, Is.Not.Null);
        Assert.That(sportEvent.Participants, Is.Empty);
        Assert.That(sportEvent.Waitlist, Is.Not.Null);
        Assert.That(sportEvent.Waitlist, Is.Empty);
        Assert.That(sportEvent.CostPerPerson, Is.EqualTo(0));
        Assert.That(sportEvent.Currency, Is.EqualTo("USD"));
        Assert.That(sportEvent.EquipmentProvided, Is.False);
        Assert.That(sportEvent.Status, Is.EqualTo(ActivityStatus.Draft));
        Assert.That(sportEvent.IsActive, Is.True);
        Assert.That(sportEvent.IsPublic, Is.True);
        Assert.That(sportEvent.IsFeatured, Is.False);
        Assert.That(sportEvent.Views, Is.EqualTo(0));
        Assert.That(sportEvent.Tags, Is.Not.Null);
        Assert.That(sportEvent.Tags, Is.Empty);
        Assert.That(sportEvent.ImageUrls, Is.Not.Null);
        Assert.That(sportEvent.ImageUrls, Is.Empty);
        Assert.That(sportEvent.CreatedAt, Is.Not.EqualTo(default(DateTime)));
        Assert.That(sportEvent.UpdatedAt, Is.Not.EqualTo(default(DateTime)));
    }

    [Test]
    public void Event_SetProperties_ShouldUpdateCorrectly()
    {
        // Arrange
        var sportEvent = new Event();
        var testDate = DateTime.UtcNow.AddDays(7);
        var testStartTime = DateTime.UtcNow.AddDays(7).AddHours(10);
        var testEndTime = DateTime.UtcNow.AddDays(7).AddHours(12);

        // Act
        sportEvent.Name = "Soccer Match";
        sportEvent.Description = "A friendly soccer match";
        sportEvent.SportType = SportType.Soccer;
        sportEvent.Category = SportCategory.TeamSport;
        sportEvent.Location = "Central Park";
        sportEvent.Address = "123 Park Ave";
        sportEvent.City = "New York";
        sportEvent.Country = "USA";
        sportEvent.Latitude = 40.7829;
        sportEvent.Longitude = -73.9654;
        sportEvent.RadiusKm = 5.0;
        sportEvent.ScheduledDate = testDate;
        sportEvent.StartTime = testStartTime;
        sportEvent.EndTime = testEndTime;
        sportEvent.Duration = 120;
        sportEvent.MaxParticipants = 20;
        sportEvent.MinParticipants = 10;
        sportEvent.SkillLevelRequired = ActivitySkillLevel.Intermediate;
        sportEvent.AgeRestriction = "18+";
        sportEvent.CostPerPerson = 15.50m;
        sportEvent.Currency = "EUR";
        sportEvent.EquipmentProvided = true;
        sportEvent.EquipmentNeeded = "Soccer cleats, shin guards";
        sportEvent.Status = ActivityStatus.Published;
        sportEvent.IsActive = true;
        sportEvent.IsPublic = true;
        sportEvent.IsFeatured = true;
        sportEvent.CreatedBy = "507f1f77bcf86cd799439011";
        sportEvent.OrganizerId = "507f1f77bcf86cd799439012";
        sportEvent.Tags = new List<string> { "soccer", "outdoor", "competitive" };

        // Assert
        Assert.That(sportEvent.Name, Is.EqualTo("Soccer Match"));
        Assert.That(sportEvent.Description, Is.EqualTo("A friendly soccer match"));
        Assert.That(sportEvent.SportType, Is.EqualTo(SportType.Soccer));
        Assert.That(sportEvent.Category, Is.EqualTo(SportCategory.TeamSport));
        Assert.That(sportEvent.Location, Is.EqualTo("Central Park"));
        Assert.That(sportEvent.Address, Is.EqualTo("123 Park Ave"));
        Assert.That(sportEvent.City, Is.EqualTo("New York"));
        Assert.That(sportEvent.Country, Is.EqualTo("USA"));
        Assert.That(sportEvent.Latitude, Is.EqualTo(40.7829));
        Assert.That(sportEvent.Longitude, Is.EqualTo(-73.9654));
        Assert.That(sportEvent.RadiusKm, Is.EqualTo(5.0));
        Assert.That(sportEvent.ScheduledDate, Is.EqualTo(testDate));
        Assert.That(sportEvent.StartTime, Is.EqualTo(testStartTime));
        Assert.That(sportEvent.EndTime, Is.EqualTo(testEndTime));
        Assert.That(sportEvent.Duration, Is.EqualTo(120));
        Assert.That(sportEvent.MaxParticipants, Is.EqualTo(20));
        Assert.That(sportEvent.MinParticipants, Is.EqualTo(10));
        Assert.That(sportEvent.SkillLevelRequired, Is.EqualTo(ActivitySkillLevel.Intermediate));
        Assert.That(sportEvent.AgeRestriction, Is.EqualTo("18+"));
        Assert.That(sportEvent.CostPerPerson, Is.EqualTo(15.50m));
        Assert.That(sportEvent.Currency, Is.EqualTo("EUR"));
        Assert.That(sportEvent.EquipmentProvided, Is.True);
        Assert.That(sportEvent.EquipmentNeeded, Is.EqualTo("Soccer cleats, shin guards"));
        Assert.That(sportEvent.Status, Is.EqualTo(ActivityStatus.Published));
        Assert.That(sportEvent.IsActive, Is.True);
        Assert.That(sportEvent.IsPublic, Is.True);
        Assert.That(sportEvent.IsFeatured, Is.True);
        Assert.That(sportEvent.CreatedBy, Is.EqualTo("507f1f77bcf86cd799439011"));
        Assert.That(sportEvent.OrganizerId, Is.EqualTo("507f1f77bcf86cd799439012"));
        Assert.That(sportEvent.Tags, Has.Count.EqualTo(3));
        Assert.That(sportEvent.Tags, Does.Contain("soccer"));
    }

    [Test]
    public void Event_ParticipantManagement_ShouldWorkCorrectly()
    {
        // Arrange
        var sportEvent = new Event
        {
            Name = "Basketball Game",
            MaxParticipants = 10,
            CurrentParticipants = 0
        };

        var participant1 = "507f1f77bcf86cd799439011";
        var participant2 = "507f1f77bcf86cd799439012";
        var participant3 = "507f1f77bcf86cd799439013";

        // Act
        sportEvent.Participants.Add(participant1);
        sportEvent.CurrentParticipants++;
        sportEvent.Participants.Add(participant2);
        sportEvent.CurrentParticipants++;
        sportEvent.Waitlist.Add(participant3);

        // Assert
        Assert.That(sportEvent.Participants, Has.Count.EqualTo(2));
        Assert.That(sportEvent.Participants, Does.Contain(participant1));
        Assert.That(sportEvent.Participants, Does.Contain(participant2));
        Assert.That(sportEvent.CurrentParticipants, Is.EqualTo(2));
        Assert.That(sportEvent.Waitlist, Has.Count.EqualTo(1));
        Assert.That(sportEvent.Waitlist, Does.Contain(participant3));
        Assert.That(sportEvent.CurrentParticipants, Is.LessThan(sportEvent.MaxParticipants));
    }
}
