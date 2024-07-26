// https://github.com/altmp/altv-ui/blob/main/src/stores/localization.ts

export interface Locale {
  code: string;
  name: string;
  rtl?: boolean;
  data: Record<string, string>;
  intlCode?: string;
  preparedStrings?: Map<string, string>;
}

export interface LocaleStore {
  dir: 'ltr' | 'rtl';
  locales: Map<string, Locale>;
  defaultLocale: string;
  currentLocale: Locale;
}