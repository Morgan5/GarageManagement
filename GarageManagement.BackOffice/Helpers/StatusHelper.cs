namespace GarageManagement.BackOffice.Helpers
{
    public static class StatusHelper
    {
        /// <summary>
        /// Retourne une description textuelle pour un statut numérique.
        /// </summary>
        /// <param name="status">Le statut numérique.</param>
        /// <returns>Une chaîne décrivant le statut.</returns>
        public static string GetStatusDescription(long status)
        {
            return status switch
            {
                0 => "En attente",
                1 => "En cours",
                2 => "Terminée",
                _ => "Inconnu"
            };
        }
    }
}