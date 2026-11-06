using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;

namespace AugatonLib.World
{
    public static class SiteDanger
    {
        // Decontamination.IsDecontaminating ne passe a vrai qu'a la fin du compte a rebours, quand il est trop tard
        // pour sortir de la zone.
        public static bool LightZoneClosesWithin(float seconds)
        {
            if (Decontamination.IsDecontaminating)
                return true;

            if (Decontamination.Status == DecontaminationController.DecontaminationStatus.Disabled)
                return false;

            foreach (DecontaminationController.DecontaminationPhase phase in DecontaminationController.Singleton.DecontaminationPhases)
            {
                if (phase.Function == DecontaminationController.DecontaminationPhase.PhaseFunction.Final)
                    return phase.TimeTrigger - Decontamination.ServerTime <= seconds;
            }

            return false;
        }
    }
}
