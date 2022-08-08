using System.Diagnostics;
using Orleans;
using Orleans.Core;
using Orleans.Providers;
using Orleans.Runtime;
using VotingContract;

namespace VotingData;

public class VoteGrain : Grain, IVoteGrain
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, int> _state;

    public VoteGrain(ILogger<VoteGrain> logger)
    {
        _logger = logger;
        _state = new Dictionary<string, int>();
    }

    public Task<Dictionary<string, int>> Get() => Task.FromResult(_state);

    public async Task AddVote(string option)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Saving vote");

        var key = option.ToLower();
        if (!_state.ContainsKey(key))
        {
            _logger.LogInformation("Created vote option {Option} and voted...", option);
            _state.Add(key, 1);
        }
        else
        {
            _logger.LogInformation("Voting for {Option}...", option);
            _state[key] += 1;
        }

        //await _state.WriteStateAsync();
        _logger.LogInformation("Saved vote in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
    }

    public async Task RemoveVote(string option)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Deleting vote option");

        var key = option.ToLower();
        if (!_state.ContainsKey(key))
        {
            _logger.LogWarning("Didn't find vote option {Option}", key);
            throw new KeyNotFoundException($"Didn't find vote option {key}");
        }
        else
        {
            _logger.LogInformation("Removed vote option {Option}...", key);
            _state.Remove(key.ToLower());
        }

        //await _state.WriteStateAsync();

        _logger.LogInformation("Deleted vote option {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
    }
}
