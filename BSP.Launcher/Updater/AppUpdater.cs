namespace BSP.Updater
{
    public static class AppUpdater
    {

        /// <summary>
        /// Проверяет, нужно ли обновление программы
        /// </summary>
        /// <param name="oldVer">Старая версия ПО</param>
        /// <param name="newVer">Новая версия ПО</param>
        /// <returns></returns>
        public static bool IsNewer(Version oldVer, Version newVer)
        {
            return newVer > oldVer;
        }
    }
}
