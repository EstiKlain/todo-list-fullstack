# שלב 1: שימוש ב-SDK של דוט-נט כדי לבנות את האפליקציה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# העתקת קבצי הפרויקט וביצוע Restore (הבאת ספריות NuGet)
COPY . ./
RUN dotnet restore

# בניית הפרויקט במצב Release
RUN dotnet publish -c Release -o out

# שלב 2: יצירת סביבת הריצה (קלה יותר מה-SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
# העתקת הקוד המוכן מהשלב הקודם
COPY --from=build-env /App/out .

# הפקודה שמפעילה את השרת
ENTRYPOINT ["dotnet", "TodoApi.dll"]