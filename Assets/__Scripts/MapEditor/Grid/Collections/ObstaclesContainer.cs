﻿using UnityEngine;

public class ObstaclesContainer : BeatmapObjectContainerCollection
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private TracksManager tracksManager;
    [SerializeField] private CountersPlusController countersPlus;

    public override BeatmapObject.ObjectType ContainerType => BeatmapObject.ObjectType.Obstacle;

    internal override void SubscribeToCallbacks()
    {
        Shader.SetGlobalFloat("_OutsideAlpha", 1f);
        Settings.NotifyBySettingName(nameof(Settings.SimplifiedObstacles), (_) => RefreshPool(true));
        Settings.NotifyBySettingName(nameof(Settings.ObstacleOpacity), (_) => RefreshPool(true));
        AudioTimeSyncController.PlayToggle += OnPlayToggle;
    }

    internal override void UnsubscribeToCallbacks()
    {
        Settings.ClearSettingNotifications(nameof(Settings.SimplifiedObstacles));
        Settings.ClearSettingNotifications(nameof(Settings.ObstacleOpacity));
        AudioTimeSyncController.PlayToggle -= OnPlayToggle;
    }

    private void OnPlayToggle(bool playing) => Shader.SetGlobalFloat("_OutsideAlpha", playing ? 0 : 1);


    protected override void OnObjectSpawned(BeatmapObject _)
        => countersPlus.UpdateStatistic(CountersPlusStatistic.Obstacles);

    protected override void OnObjectDelete(BeatmapObject _)
        => countersPlus.UpdateStatistic(CountersPlusStatistic.Obstacles);

    public override BeatmapObjectContainer CreateContainer()
        => BeatmapObstacleContainer.SpawnObstacle(null, tracksManager, ref obstaclePrefab);

    protected override void UpdateContainerData(BeatmapObjectContainer con, BeatmapObject obj)
    {
        var obstacle = con as BeatmapObstacleContainer;

        var track = tracksManager.GetTrackAtTime(obj.Time);
        track.AttachContainer(con);

        obstacle.SetAlpha(Settings.Instance.ObstacleOpacity);
    }
}
