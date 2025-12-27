
-- Unique per user (ignore global rows where UserId IS NULL)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'UX_Categories_UserId_Name'
      AND object_id = OBJECT_ID('dbo.Categories')
)
BEGIN
    CREATE UNIQUE INDEX UX_Categories_UserId_Name
    ON dbo.Categories(UserId, Name)
    WHERE UserId IS NOT NULL;
END

-- Helpful index for filtering by UserId
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_Categories_UserId'
      AND object_id = OBJECT_ID('dbo.Categories')
)
BEGIN
    CREATE INDEX IX_Categories_UserId
    ON dbo.Categories(UserId);
END
