using AutoFixture;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Keys;
using Xunit;

namespace OCS.TeamAssistant.Appraiser.DomainTests;

public sealed class AssessmentSessionTests
{
	private readonly Fixture _fixture = new ();
	private readonly Participant _moderator;
	private readonly AssessmentSession _target;

	public AssessmentSessionTests()
	{
		_moderator = _fixture.Create<Participant>();
		_target = new (_fixture.Create<long>(), _moderator);
	}

	[Theory]
	[InlineData(AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp2)]
	[InlineData(AssessmentValue.Sp3)]
	[InlineData(AssessmentValue.Sp5)]
	[InlineData(AssessmentValue.Sp8)]
	[InlineData(AssessmentValue.Sp13)]
	[InlineData(AssessmentValue.Sp21)]
	public void Estimate_Value_ReturnsValue(AssessmentValue value)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>());
		_target.AddStoryForEstimate(new(_moderator, _fixture.Create<int>()));
		_target.Estimate(_moderator, value);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)value, actual);
	}

	[Theory]
	[InlineData(AssessmentValue.Sp1, AssessmentValue.Sp21)]
	[InlineData(AssessmentValue.Sp2, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp3, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp5, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp8, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp13, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp21, AssessmentValue.Sp1)]
	public void Reset_SecondValue_ReturnsSecondValue(AssessmentValue firstValue, AssessmentValue secondValue)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());
		_target.Connect(_fixture.Create<ParticipantId>(), _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>());
		_target.AddStoryForEstimate(new(_moderator, _fixture.Create<int>()));
		_target.Estimate(_moderator, firstValue);

		_target.Estimate(_moderator, secondValue);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)secondValue, actual);
	}

	[Theory]
	[InlineData(AssessmentValue.Sp1, AssessmentValue.Sp21)]
	[InlineData(AssessmentValue.Sp2, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp3, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp5, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp8, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp13, AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp21, AssessmentValue.Sp1)]
	public void Reset_ResetValue_ReturnsSecondValue(AssessmentValue firstValue, AssessmentValue secondValue)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>());
		_target.AddStoryForEstimate(new(_moderator, _fixture.Create<int>()));
		_target.Estimate(_moderator, firstValue);

		_target.Reset(_moderator.Id);
		_target.Estimate(_moderator, secondValue);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)secondValue, actual);
	}

	[Theory]
	[InlineData(AssessmentValue.Sp1)]
	[InlineData(AssessmentValue.Sp2)]
	[InlineData(AssessmentValue.Sp3)]
	[InlineData(AssessmentValue.Sp5)]
	[InlineData(AssessmentValue.Sp8)]
	[InlineData(AssessmentValue.Sp13)]
	[InlineData(AssessmentValue.Sp21)]
	public void Estimate_SecondStory_ReturnsValue(AssessmentValue value)
	{
		_target.Activate(_moderator.Id, _fixture.Create<string>());

		_target.StartStorySelection(_moderator.Id);
		_target.StorySelected(_moderator.Id, _fixture.Create<string>());
		_target.AddStoryForEstimate(new(_moderator, _fixture.Create<int>()));
		_target.Estimate(_moderator, value);

		var secondStory = _fixture.Create<string>();
		_target.StartStorySelection(_moderator.Id);
		_target.StorySelected(_moderator.Id, secondStory);
		_target.AddStoryForEstimate(new(_moderator, _fixture.Create<int>()));
		_target.Estimate(_moderator, value);

		var actual = _target.CurrentStory.GetTotal();

		Assert.Equal((decimal?)value, actual);
		Assert.Equal(secondStory, _target.CurrentStory.Title);
	}
}