ARG DOTNET_VERSION=6.0

#-------------------------------------------------------------------------------
# Сборка базового образа среды выполнения
FROM core/astralinux:1.7-minbase AS base
ARG DOTNET_VERSION
# https://github.com/hadolint/hadolint/wiki/DL4006
SHELL ["/bin/bash", "-o", "pipefail", "-c"]

# 1. Интернет-репозитории Astra Linux Special Edition x.7
#    - https://wiki.astralinux.ru/pages/viewpage.action?pageId=158598882
# 2. Репозитории Astra Linux Special Edition x.7: структура, особенности подключения и использования
#    - https://wiki.astralinux.ru/pages/viewpage.action?pageId=149062354
# 3. Использование стороннего программного обеспечения в аттестованных информационных системах,
#    функционирующих под управлением Astra Linux Special Edition
#    - https://wiki.astralinux.ru/pages/viewpage.action?pageId=63802854
#
# Основной репозиторий
#  "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-main/ 1.7_x86-64 main contrib non-free"
# Оперативные обновления основного репозитория
#  "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-update/ 1.7_x86-64 main contrib non-free"
# Базовый репозиторий
#  "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-base/ 1.7_x86-64 main contrib non-free"
# Расширенный репозиторий (не сертифицирован ФСТЭК)
#  "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-extended/ 1.7_x86-64 main contrib non-free"

WORKDIR /tmp
RUN echo "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-main/ 1.7_x86-64 main contrib non-free" > /etc/apt/sources.list \
    && echo "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-update/ 1.7_x86-64 main contrib non-free" >>  /etc/apt/sources.list \
    && echo "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-base/ 1.7_x86-64 main contrib non-free" >>  /etc/apt/sources.list \
    && echo "deb http://dl.astralinux.ru/astra/stable/1.7_x86-64/repository-extended/ 1.7_x86-64 main contrib non-free" >>  /etc/apt/sources.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        adduser \
        apt-transport-https \
        bash \
        ca-certificates \
        curl \
        locales \
    && echo "deb http://packages.lab50.net/mono alse17 main" > /etc/apt/sources.list.d/mono.list \
    && curl -fsSL http://packages.lab50.net/lab50.asc | apt-key add - \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        aspnetcore-runtime-${DOTNET_VERSION} \
        dotnet-host-${DOTNET_VERSION} \
    && echo 'ru_RU.UTF-8 UTF-8' >> /etc/locale.gen \
    && echo 'en_US.UTF-8 UTF-8' >> /etc/locale.gen \
    && /usr/sbin/locale-gen \
    && /usr/sbin/update-locale en_US.UTF-8 \
    && adduser --home /app --group --system --uid 10001 appuser \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

#-------------------------------------------------------------------------------
# Сборка образа SDK и выполнение сборки проекта
FROM base AS publish
ARG DOTNET_VERSION
ENV PATH $PATH:/root/.dotnet/tools
# Отключить телеметрию https://aka.ms/dotnet-cli-telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

RUN apt-get update\
    && apt-get install -y --no-install-recommends dotnet-sdk-${DOTNET_VERSION}

WORKDIR /src
COPY . .
WORKDIR /src/AstraLinux.Web
RUN dotnet publish "AstraLinux.Web.csproj" -c Release -o /app/publish

#-------------------------------------------------------------------------------
# Сборка финального образа с готовым приложением
FROM base AS final
WORKDIR /app
EXPOSE 8080
RUN rm -rf /tmp/*
ENV ASPNETCORE_URLS="http://+:8080"
COPY --chown=appuser --from=publish /app/publish .

# Выполнение приложения в контейнере должно осуществляться непривилегированным пользователем.
USER appuser

ENTRYPOINT ["dotnet", "AstraLinux.Web.dll"]