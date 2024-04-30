# ASP.NET CORE HealthChecks General

## Overview

I have been advocating for the use of the AspNetCore Diagnostics HealthChecks [Health Checks GitHub Page](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks) in the company that I work. We have developed a small project to monitor the necessary systems, solely focusing on that aspect. However, after a few weeks, we started receiving requests to include checks for other systems as well. It became apparent that the existing code wasn't sufficient for our needs. The purpose of this Repro is to showcase my research and work in implementing additional checks that we believe would be valuable.

Essentially, this revolves around identifying what I feel is lacking and suggesting improvements to the existing code base.

## NuGet package

Status: In process of finding out how to implement this is currently not a priority.

## Hangfire - (started)

Although there is already a Hangfire package included in the existing HealthChecks, it doesn't offer many of the features that I would like to see.

* Identifying if any servers are down - Complete
* Indicating the status of specific servers (last error logged?)
* Tracking the count of failed jobs over specific timeframes (24 hours, week, month)
* Providing details about recurring jobs

Initially, I plan to proceed with implementing these features without relying on Hangfire.

## Sonar Cloud

Create health checks to use the Sonar Cloud API to check the status of projects.

* Allow specific branches to be specified
* Overall Code
    1. Security
    2. Reliability
    3. Maintainability
    4. Duplications
    5. Security Hotspots
* New Core
    1. New Issues
    2. Duplications
    3. Security Hotspots

As the Sonar Cloud API is rate limited may need to cache the response, this should be an optional parameter.

### Unit tests

You are required to create user secrets for the unit tests to run. Goto the sonar project to view the readme for more information.