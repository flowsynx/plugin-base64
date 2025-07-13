## FlowSynx Base64 Plugin

The **FlowSynx Base64 Plugin** is a built-in, plug-and-play integration for the FlowSynx automation engine. It enables encoding, decoding, and validation of Base64-encoded data within workflows, with no custom coding required.

This plugin is automatically installed by the FlowSynx engine when selected in the workflow builder. It is not intended for standalone developer usage outside the FlowSynx platform.

---

## Purpose

The Base64 Plugin allows FlowSynx users to:

- Encode raw data (strings or byte arrays) into Base64 format.
- Decode Base64-encoded data back into its original form.
- Validate whether a given string is properly Base64-encoded.

It integrates seamlessly into FlowSynx no-code/low-code workflows for data transformation and validation tasks.

---

## Supported Operations

- **encode**: Encodes the `Data` parameter into a Base64 string.
- **decode**: Decodes a Base64 string in `Data` back into its original form.
- **valid**: Checks if the `Data` parameter is a valid Base64-encoded string and returns a boolean result.

---

## Input Parameters

The plugin accepts the following parameters:

- `Operation` (string): **Required.** The type of operation to perform. Supported values are `encode`, `decode`, and `valid`.
- `Data` (object): **Required.** The input data to process. For `encode`, this can be a string or byte array. For `decode` and `valid`, this must be a Base64-encoded string.

### Example input

```json
{
  "Operation": "encode",
  "Data": "Hello, FlowSynx!"
}
```

---

## Operation Examples

### encode Operation

**Input Parameters:**

```json
{
  "Operation": "encode",
  "Data": "Hello, FlowSynx!"
}
```

**Output:**

```json
"SGVsbG8sIEZsb3dTeW54IQ=="
```

---

### decode Operation

**Input Parameters:**

```json
{
  "Operation": "decode",
  "Data": "SGVsbG8sIEZsb3dTeW54IQ=="
}
```

**Output:**

```json
"Hello, FlowSynx!"
```

---

### valid Operation

**Input Parameters:**

```json
{
  "Operation": "valid",
  "Data": "SGVsbG8sIEZsb3dTeW54IQ=="
}
```

**Output:**

```json
true
```

**Example with invalid Base64 string:**

```json
{
  "Operation": "valid",
  "Data": "Not a valid Base64!!"
}
```

**Output:**

```json
false
```

---

## Example Use Case in FlowSynx

1. Add the Base64 plugin to your FlowSynx workflow.
2. Set `Operation` to one of: `encode`, `decode`, or `valid`.
3. Provide input data in `Data`.
4. Use the plugin output downstream in your workflow for further processing or decision-making.

---

## Debugging Tips

- If `decode` fails, ensure the `Data` parameter contains a valid Base64-encoded string.
- The `valid` operation can be used prior to `decode` to avoid runtime errors.
- For non-UTF8 encoded binary data, make sure your workflow handles byte arrays correctly.

---

## Security Notes

- No data is persisted unless explicitly configured.
- All operations run in a secure sandbox within FlowSynx.
- Only authorized platform users can view or modify configurations.

---

## License

© FlowSynx. All rights reserved.3