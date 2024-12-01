namespace GoTF.GameLoading
{
    public interface ISaveable
    {
        public void SaveToJson();

        public void LoadFromJson();
    }
}
