namespace Haven
{
    interface ICloneable<T>
    {
        /// <summary>
        /// Performs a deep copy of the object.
        /// </summary>
        /// <returns>A new copy of the object.</returns>
        T Clone();
    }
}
