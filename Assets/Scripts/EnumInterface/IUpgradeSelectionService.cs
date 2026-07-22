using System;

public interface IUpgradeSelectionService {
    event Action<UpgradeOffer[]> UpgradeOffersReady;

    void SelectUpgrade( string upgradeId );
}
