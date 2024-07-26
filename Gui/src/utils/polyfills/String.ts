export const normalizeString = (type: 'path', value: string): string => {
  switch (type) {
    default: throw new ReferenceError('Unknown normalization type');

    // Removes directory path and file extension from the given value
    case 'path': return value.replace(/^.*[\\/]|(\..+)$/g, '');
  }
};

export const stringContains = (string: string, substring: string, position: number = 0): boolean => string.indexOf(substring, position) !== -1;
export const isStringNumericHostname = (hostname: string): boolean => /^\d+(\.\d+)*$/.test(hostname);