using System;

public interface ILevelProgressSource {
    int CurrentLevel { get; }

    event Action<int, float> ExperienceChanged;
    event Action<int> LevelChanged;
    event Action ExperiencePanelRequested;
}
