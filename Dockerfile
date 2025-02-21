FROM mcr.microsoft.com/dotnet/sdk:7.0 as backend

WORKDIR /src

COPY ./GithubNotificationsTelegramBot.sln ./
COPY ./GithubNotificationsTelegramBot/GithubNotificationsTelegramBot.csproj ./GithubNotificationsTelegramBot/

RUN dotnet restore --packages /nuget
COPY ./ .

WORKDIR /src/./GithubNotificationsTelegramBot/
RUN ls -la && dotnet publish --output /out --no-restore -v m

FROM mcr.microsoft.com/dotnet/aspnet:7.0

COPY --from=backend /out /app
WORKDIR /app
EXPOSE 80
EXPOSE 8080
