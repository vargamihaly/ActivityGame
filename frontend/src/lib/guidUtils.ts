/**
 * guidUtils.ts
 * Utility functions for working with GUIDs.
 */

/**
 * Regular expression for validating GUIDs.
 * Matches the pattern: 8-4-4-4-12 hexadecimal characters.
 */
const GUID_REGEX = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

/**
 * Validates if a given string is a valid GUID.
 * @param guid The string to validate as a GUID.
 * @returns True if the string is a valid GUID, false otherwise.
 */
export const isValidGuid = (guid: string): boolean => {
    return GUID_REGEX.test(guid);
};

/**
 * Throws an error if the provided string is not a valid GUID.
 * @param guid The string to validate as a GUID.
 * @throws Error if the string is not a valid GUID.
 */
export const validateGuid = (guid: string): void => {
    if (!isValidGuid(guid)) {
        throw new Error(`Invalid GUID: ${guid}`);
    }
};

/**
 * Generates a random GUID.
 * Note: This is a simple implementation and may not be suitable for cryptographic purposes.
 * @returns A randomly generated GUID string.
 */
export const generateGuid = (): string => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0,
            v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};

/**
 * Formats a string as a GUID if it's valid.
 * @param input The string to format as a GUID.
 * @returns The formatted GUID if valid, or null if invalid.
 */
export const formatAsGuid = (input: string): string | null => {
    const clean = input.replace(/[^0-9a-fA-F]/g, '');
    if (clean.length !== 32) return null;

    const parts = [
        clean.substr(0, 8),
        clean.substr(8, 4),
        clean.substr(12, 4),
        clean.substr(16, 4),
        clean.substr(20, 12)
    ];

    const formatted = parts.join('-');
    return isValidGuid(formatted) ? formatted : null;
};