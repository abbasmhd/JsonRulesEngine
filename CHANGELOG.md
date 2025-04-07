# Changelog

All notable changes to the JsonRulesEngine project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Enhanced fact caching system
  - Global cache control through `EngineOptions` and `AlmanacOptions`
  - Per-fact cache configuration with `FactOptions`
  - Time-based cache expiration with `CacheExpirationInSeconds`
  - LRU (Least Recently Used) cache eviction strategy
  - Cache size limits with `CacheMaxSize`
  - Cache invalidation methods: `ClearCache()` and `InvalidateCache(factId)`
  - Comprehensive test coverage for caching functionality
  - Updated documentation with examples and best practices

### Changed
- Improved performance for rule evaluation with repeated fact usage
- Enhanced `IAlmanac` interface with caching capabilities

### Fixed
- No fixes in this release 