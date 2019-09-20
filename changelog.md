# Changelog

## 0.1.0
- Initial release

## 0.2.0
- Minimum required version of .NET Standard raised to 2.1.
- `Fody.Equals` and `Messerli.Utility` are no longer pulled in as dependencies.
- `FileOpeningBuilder` is now immutable: Modifying methods such as like `.Read()` and `.Write()` now
  return a copy instead of changing the builder's state.

