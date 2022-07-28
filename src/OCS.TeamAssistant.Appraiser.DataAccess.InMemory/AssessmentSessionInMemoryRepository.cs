using System.Collections.Concurrent;
using OCS.TeamAssistant.Appraiser.Application.Contracts;
using OCS.TeamAssistant.Appraiser.Domain;
using OCS.TeamAssistant.Appraiser.Domain.Exceptions;
using OCS.TeamAssistant.Appraiser.Domain.Keys;

namespace OCS.TeamAssistant.Appraiser.DataAccess.InMemory;

internal sealed class AssessmentSessionInMemoryRepository : IAssessmentSessionRepository
{
    private ConcurrentBag<AssessmentSession> _store = new ();

    public Task<AssessmentSession?> Find(AssessmentSessionId assessmentSessionId, CancellationToken cancellationToken)
    {
        if (assessmentSessionId is null)
            throw new ArgumentNullException(nameof(assessmentSessionId));

        return Task.FromResult(_store.SingleOrDefault(i => i.Id == assessmentSessionId));
    }

    public Task<AssessmentSession?> Find(ParticipantId participantId, CancellationToken cancellationToken)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        var assessmentSessions = _store.Where(i => i.Participants.Any(a => a.Id.Equals(participantId))).ToArray();

        return assessmentSessions.Length switch
        {
            0 => Task.FromResult<AssessmentSession?>(default),
            1 => Task.FromResult<AssessmentSession?>(assessmentSessions[0]),
            _ => throw new AppraiserUserException(
                MessageId.ActiveSessionsFound,
                assessmentSessions.Length,
                participantId.Value)
        };
    }

    public Task Add(AssessmentSession assessmentSession, CancellationToken cancellationToken)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        _store.Add(assessmentSession);

        return Task.CompletedTask;
    }

    public Task Update(AssessmentSession assessmentSession, CancellationToken cancellationToken)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        return Task.CompletedTask;
    }

    public Task Remove(AssessmentSession assessmentSession, CancellationToken cancellationToken)
    {
        _store = new(_store.Where(s => s != assessmentSession));

        return Task.CompletedTask;
    }
}